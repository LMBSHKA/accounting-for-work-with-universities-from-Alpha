using Application.Abstractions.Persistence;
using Application.Abstractions.Students;
using Application.Students.Models;
using Entities.Models;
using TeamResult = Application.Students.Models.TeamResult;

namespace Application.Students.Services;

public class StudentService(IUnitOfWork unitOfWork) : IStudentService
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task<StudentResult?> CreateAsync(CreateStudentCommand command, CancellationToken cancellationToken = default)
	{
		var fullName = NormalizeRequired(command.FullName);
		var email = NormalizeRequired(command.Email).ToLower();
		var roleInTeam = NormalizeOptional(command.RoleInTeam);

		if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
		{
			return null;
		}

		var existingStudent = await _unitOfWork.StudentProfiles.GetByEmailAsync(email, cancellationToken);
		if (existingStudent is not null)
		{
			return null;
		}

		var student = new StudentProfile
		{
			Id = Guid.NewGuid(),
			FullName = fullName,
			Email = email,
			RoleInTeam = roleInTeam,
			UpdatedAt = DateTime.UtcNow
		};

		await _unitOfWork.StudentProfiles.AddAsync(student, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Map(student);
	}

	public async Task<TeamResult?> CreateTeamAsync(CreateTeamCommand command, CancellationToken cancellationToken = default)
	{
		var name = NormalizeRequired(command.Name);
		var skills = NormalizeOptional(command.Skills);
		var curatorId = command.CuratorId.HasValue && command.CuratorId.Value != Guid.Empty
			? command.CuratorId.Value
			: (Guid?)null;
		var studentProfileIds = command.StudentProfileIds
			.Where(id => id != Guid.Empty)
			.Distinct()
			.ToList();

		if (command.ProjectId == Guid.Empty || command.CreatedByUserId == Guid.Empty || string.IsNullOrWhiteSpace(name) || studentProfileIds.Count == 0)
		{
			return null;
		}

		var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken);
		if (project is null)
		{
			return null;
		}

		User? curator = null;
		if (curatorId.HasValue)
		{
			curator = await _unitOfWork.Users.GetByIdAsync(curatorId.Value, cancellationToken);
			if (curator is null)
			{
				return null;
			}
		}

		var teamWithSameNameExists = _unitOfWork.Teams
			.Query()
			.Any(team => team.ProjectId == command.ProjectId && team.Name == name);

		if (teamWithSameNameExists)
		{
			return null;
		}

		var students = new List<StudentProfile>();
		foreach (var studentProfileId in studentProfileIds)
		{
			var student = await _unitOfWork.StudentProfiles.GetByIdAsync(studentProfileId, cancellationToken);
			if (student is null)
			{
				return null;
			}

			students.Add(student);
		}

		var now = DateTime.UtcNow;
		var team = new Team
		{
			Id = Guid.NewGuid(),
			ProjectId = command.ProjectId,
			Name = name,
			Skills = skills,
			CuratorId = curatorId,
			Curator = curator,
			CreatedByUserId = command.CreatedByUserId,
			CreatedAt = now
		};

		await _unitOfWork.Teams.AddAsync(team, cancellationToken);

		foreach (var student in students)
		{
			var member = new TeamMember
			{
				Id = Guid.NewGuid(),
				TeamId = team.Id,
				StudentsProfileId = student.Id,
				Team = team,
				StudentsProfile = student
			};

			team.Members.Add(member);
			await _unitOfWork.TeamMembers.AddAsync(member, cancellationToken);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Map(team);
	}

	public Task<GetStudentsResult> GetStudentsAsync(GetStudentsQuery query, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.StudentProfiles.GetStudentsAsync(query, cancellationToken);
	}

	public Task<GetTeamsResult> GetTeamsAsync(GetTeamsQuery query, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.Teams.GetTeamsAsync(query, cancellationToken);
	}

	public Task<GetCuratorsResult> GetCuratorsAsync(GetCuratorsQuery query, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.Teams.GetCuratorsAsync(query, cancellationToken);
	}

	public Task<TeamDetailsResult?> GetTeamDetailsAsync(Guid teamId, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.Teams.GetTeamDetailsAsync(teamId, cancellationToken);
	}

	public async Task<int> DeleteStudentsAsync(IEnumerable<Guid> studentIds, CancellationToken cancellationToken = default)
	{
		var ids = studentIds
			.Where(id => id != Guid.Empty)
			.Distinct()
			.ToList();

		if (ids.Count == 0)
		{
			return 0;
		}

		var teamMembers = _unitOfWork.TeamMembers
			.Query()
			.Where(member => ids.Contains(member.StudentsProfileId))
			.ToList();

		foreach (var teamMember in teamMembers)
		{
			_unitOfWork.TeamMembers.Remove(teamMember);
		}

		var finalMemberScores = _unitOfWork.FinalMemberScores
			.Query()
			.Where(score => ids.Contains(score.StudentProfileId))
			.ToList();

		foreach (var finalMemberScore in finalMemberScores)
		{
			_unitOfWork.FinalMemberScores.Remove(finalMemberScore);
		}

		var students = _unitOfWork.StudentProfiles
			.Query()
			.Where(student => ids.Contains(student.Id))
			.ToList();

		foreach (var student in students)
		{
			_unitOfWork.StudentProfiles.Remove(student);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return students.Count;
	}

	public async Task<int> DeleteTeamsAsync(IEnumerable<Guid> teamIds, CancellationToken cancellationToken = default)
	{
		var ids = teamIds
			.Where(id => id != Guid.Empty)
			.Distinct()
			.ToList();

		if (ids.Count == 0)
		{
			return 0;
		}

		var teams = _unitOfWork.Teams
			.Query()
			.Where(team => ids.Contains(team.Id))
			.ToList();

		foreach (var team in teams)
		{
			_unitOfWork.Teams.Remove(team);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return teams.Count;
	}

	private static StudentResult Map(StudentProfile student)
	{
		return new StudentResult
		{
			Id = student.Id,
			FullName = student.FullName,
			Email = student.Email,
			RoleInTeam = student.RoleInTeam,
			UpdatedAt = student.UpdatedAt
		};
	}

	private static TeamResult Map(Team team)
	{
		return new TeamResult
		{
			Id = team.Id,
			ProjectId = team.ProjectId,
			Name = team.Name,
			Skills = team.Skills,
			CuratorId = team.CuratorId,
			CuratorFullName = team.Curator?.FullName,
			CreatedByUserId = team.CreatedByUserId,
			CreatedAt = team.CreatedAt,
			UpdatedAt = team.UpdatedAt,
			Members = team.Members.Select(member => new TeamMemberResult
			{
				Id = member.Id,
				StudentProfileId = member.StudentsProfileId,
				FullName = member.StudentsProfile?.FullName ?? string.Empty,
				Email = member.StudentsProfile?.Email ?? string.Empty,
				RoleInTeam = member.StudentsProfile?.RoleInTeam
			}).ToList()
		};
	}

	private static string NormalizeRequired(string value)
	{
		return value.Trim();
	}

	private static string? NormalizeOptional(string? value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
	}
}
