using API.Contracts.Students;
using Application.Abstractions.Students;
using Application.Students.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/student")]
[Authorize]
public class StudentsController(IStudentService studentService) : ControllerBase
{
	private readonly IStudentService _studentService = studentService;

	[EndpointSummary("Создание студента")]
	[HttpPost]
	public async Task<ActionResult<StudentResponse>> Create(
		[FromBody] CreateStudentRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var student = await _studentService.CreateAsync(new CreateStudentCommand
		{
			FullName = request.FullName,
			Email = request.Email,
			RoleInTeam = request.RoleInTeam
		}, cancellationToken);

		if (student is null)
		{
			return Conflict(new { message = "Student with this email already exists or request data is invalid." });
		}

		return Ok(new StudentResponse
		{
			Id = student.Id,
			FullName = student.FullName,
			Email = student.Email,
			RoleInTeam = student.RoleInTeam,
			UpdatedAt = student.UpdatedAt
		});
	}


	[EndpointSummary("Список студентов")]
	[HttpPost("list")]
	public async Task<ActionResult<GetStudentsResponse>> GetStudents(
		[FromBody] GetStudentsRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var result = await _studentService.GetStudentsAsync(new GetStudentsQuery
		{
			Search = request.Search,
			Filter = request.Filter,
			Offset = request.Offset,
			Limit = request.Limit
		}, cancellationToken);

		return Ok(new GetStudentsResponse
		{
			Items = result.Items.Select(student => new StudentListItemResponse
			{
				Id = student.Id,
				FullName = student.FullName,
				Email = student.Email,
				RoleInTeam = student.RoleInTeam,
				TeamNames = student.TeamNames,
				UpdatedAt = student.UpdatedAt
			}).ToList(),
			TotalCount = result.TotalCount,
			Offset = result.Offset,
			Limit = result.Limit,
			LoadedCount = result.LoadedCount,
			HasMore = result.HasMore,
			NextOffset = result.NextOffset
		});
	}

	[EndpointSummary("Список команд")]
	[HttpPost("team/list")]
	public async Task<ActionResult<GetTeamsResponse>> GetTeams(
		[FromBody] GetTeamsRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var result = await _studentService.GetTeamsAsync(new GetTeamsQuery
		{
			Search = request.Search,
			ProjectId = request.ProjectId,
			Filter = request.Filter,
			Offset = request.Offset,
			Limit = request.Limit
		}, cancellationToken);

		return Ok(new GetTeamsResponse
		{
			Items = result.Items.Select(team => new TeamListItemResponse
			{
				Id = team.Id,
				ProjectId = team.ProjectId,
				ProjectTitle = team.ProjectTitle,
				ProjectStatus = team.ProjectStatus,
				Name = team.Name,
				Skills = team.Skills,
				CreatedAt = team.CreatedAt,
				UpdatedAt = team.UpdatedAt,
				Members = team.Members.Select(member => new TeamMemberResponse
				{
					Id = member.Id,
					StudentProfileId = member.StudentProfileId,
					FullName = member.FullName,
					Email = member.Email,
					RoleInTeam = member.RoleInTeam
				}).ToList()
			}).ToList(),
			TotalCount = result.TotalCount,
			Offset = result.Offset,
			Limit = result.Limit,
			LoadedCount = result.LoadedCount,
			HasMore = result.HasMore,
			NextOffset = result.NextOffset
		});
	}

	/// <summary>
	/// Подробные данные команды для страницы команды: участники, стек, проект, описание проекта, количество команд проекта и итерации.
	/// </summary>
	[EndpointSummary("Карточка команды")]
	[HttpGet("team/{teamId:guid}")]
	public async Task<ActionResult<TeamDetailsResponse>> GetTeamDetails(Guid teamId, CancellationToken cancellationToken)
	{
		var team = await _studentService.GetTeamDetailsAsync(teamId, cancellationToken);
		if (team is null)
		{
			return NotFound(new { message = "Team was not found." });
		}

		return Ok(new TeamDetailsResponse
		{
			Id = team.Id,
			ProjectId = team.ProjectId,
			Name = team.Name,
			Skills = team.Skills,
			CreatedAt = team.CreatedAt,
			UpdatedAt = team.UpdatedAt,
			Project = new TeamProjectDetailsResponse
			{
				Id = team.Project.Id,
				Title = team.Project.Title,
				Description = team.Project.Description,
				Goal = team.Project.Goal,
				Mvp = team.Project.Mvp,
				EvaluationCriteria = team.Project.EvaluationCriteria,
				Deadline = team.Project.Deadline,
				Status = team.Project.Status,
				TeamsCount = team.Project.TeamsCount
			},
			Members = team.Members.Select(member => new TeamMemberResponse
			{
				Id = member.Id,
				StudentProfileId = member.StudentProfileId,
				FullName = member.FullName,
				Email = member.Email,
				RoleInTeam = member.RoleInTeam
			}).ToList(),
			Iterations = team.Iterations.Select(iteration => new TeamIterationResponse
			{
				Id = iteration.Id,
				Name = iteration.Name,
				StartOn = iteration.StartOn,
				EndOn = iteration.EndOn
			}).ToList()
		});
	}

	[EndpointSummary("Создание команды")]
	[HttpPost("team")]
	public async Task<ActionResult<TeamResponse>> CreateTeam(
		[FromBody] CreateTeamRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (!Guid.TryParse(userIdValue, out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		var team = await _studentService.CreateTeamAsync(new CreateTeamCommand
		{
			ProjectId = request.ProjectId,
			Name = request.Name,
			Skills = request.Skills,
			CreatedByUserId = userId,
			StudentProfileIds = request.StudentProfileIds
		}, cancellationToken);

		if (team is null)
		{
			return Conflict(new { message = "Team with this name already exists in project, project or students were not found, or request data is invalid." });
		}

		return Ok(new TeamResponse
		{
			Id = team.Id,
			ProjectId = team.ProjectId,
			Name = team.Name,
			Skills = team.Skills,
			CreatedByUserId = team.CreatedByUserId,
			CreatedAt = team.CreatedAt,
			UpdatedAt = team.UpdatedAt,
			Members = team.Members.Select(member => new TeamMemberResponse
			{
				Id = member.Id,
				StudentProfileId = member.StudentProfileId,
				FullName = member.FullName,
				Email = member.Email,
				RoleInTeam = member.RoleInTeam
			}).ToList()
		});
	}

	[EndpointSummary("Удаление студентов")]
	[HttpDelete]
	public async Task<IActionResult> DeleteStudents(
	[FromBody] List<Guid> ids,
	CancellationToken cancellationToken)
	{
		if (ids is null || ids.Count == 0)
		{
			return BadRequest(new { message = "Student ids list is required." });
		}

		var deletedCount = await _studentService.DeleteStudentsAsync(ids, cancellationToken);

		return Ok(new { deletedCount });
	}

	[EndpointSummary("Удаление команд")]
	[HttpDelete("team")]
	public async Task<IActionResult> DeleteTeams(
		[FromBody] List<Guid> ids,
		CancellationToken cancellationToken)
	{
		if (ids is null || ids.Count == 0)
		{
			return BadRequest(new { message = "Team ids list is required." });
		}

		var deletedCount = await _studentService.DeleteTeamsAsync(ids, cancellationToken);

		return Ok(new { deletedCount });
	}
}
