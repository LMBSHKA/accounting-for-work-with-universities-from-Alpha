using Application.Abstractions.Persistence;
using Application.Students.Models;
using Entities.enums;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TeamRepository(AppDbContext dbContext) : Repository<Team>(dbContext), ITeamRepository
{
	public async Task<GetTeamsResult> GetTeamsAsync(GetTeamsQuery request, CancellationToken cancellationToken = default)
	{
		var offset = Math.Max(0, request.Offset);
		var limit = Math.Clamp(request.Limit, 1, 100);

		var query = ApplyTeamListFilters(DbSet.AsNoTracking(), request.Search, request.Filter, request.ProjectId);
		var totalCount = await query.CountAsync(cancellationToken);

		var teams = await query
			.OrderBy(team => team.Name)
			.ThenByDescending(team => team.CreatedAt)
			.Skip(offset)
			.Take(limit)
			.Select(team => new TeamListItemResult
			{
				Id = team.Id,
				ProjectId = team.ProjectId,
				ProjectTitle = team.Project == null ? null : team.Project.Title,
				ProjectStatus = team.Project == null ? null : team.Project.Status,
				Name = team.Name,
				Skills = team.Skills,
				FileUrl = team.FileUrl,
				CuratorId = team.CuratorId,
				CuratorFullName = team.Curator == null ? null : team.Curator.FullName,
				CreatedAt = team.CreatedAt,
				UpdatedAt = team.UpdatedAt,
				Members = team.Members
					.OrderBy(member => member.StudentsProfile == null ? string.Empty : member.StudentsProfile.FullName)
					.Select(member => new TeamMemberResult
					{
						Id = member.Id,
						StudentProfileId = member.StudentsProfileId,
						FullName = member.StudentsProfile == null ? string.Empty : member.StudentsProfile.FullName,
						Email = member.StudentsProfile == null ? string.Empty : member.StudentsProfile.Email,
						RoleInTeam = member.StudentsProfile == null ? null : member.StudentsProfile.RoleInTeam
					})
					.ToList()
			})
			.ToListAsync(cancellationToken);

		var loadedCount = offset + teams.Count;

		return new GetTeamsResult
		{
			Items = teams,
			TotalCount = totalCount,
			Offset = offset,
			Limit = limit,
			LoadedCount = loadedCount,
			HasMore = loadedCount < totalCount,
			NextOffset = loadedCount < totalCount ? loadedCount : null
		};
	}

	public async Task<GetCuratorsResult> GetCuratorsAsync(GetCuratorsQuery request, CancellationToken cancellationToken = default)
	{
		var offset = Math.Max(0, request.Offset);
		var limit = Math.Clamp(request.Limit, 1, 100);

		var query = DbContext.Users
			.AsNoTracking()
			.Where(user => user.CuratedTeams.Any());

		if (!string.IsNullOrWhiteSpace(request.Search))
		{
			var normalizedSearch = request.Search.Trim().ToLower();
			query = query.Where(user =>
				user.FullName.ToLower().Contains(normalizedSearch) ||
				user.Email.ToLower().Contains(normalizedSearch));
		}

		var totalCount = await query.CountAsync(cancellationToken);

		var curators = await query
			.OrderBy(user => user.FullName)
			.ThenBy(user => user.Email)
			.Skip(offset)
			.Take(limit)
			.Select(user => new CuratorListItemResult
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				SystemRole = user.SystemRole,
				Teams = user.CuratedTeams
					.OrderBy(team => team.Name)
					.Select(team => new CuratorTeamResult
					{
						Id = team.Id,
						ProjectId = team.ProjectId,
						Name = team.Name,
						FileUrl = team.FileUrl,
						ProjectTitle = team.Project == null ? null : team.Project.Title
					})
					.ToList()
			})
			.ToListAsync(cancellationToken);

		var loadedCount = offset + curators.Count;

		return new GetCuratorsResult
		{
			Items = curators,
			TotalCount = totalCount,
			Offset = offset,
			Limit = limit,
			LoadedCount = loadedCount,
			HasMore = loadedCount < totalCount,
			NextOffset = loadedCount < totalCount ? loadedCount : null
		};
	}

	public async Task<TeamDetailsResult?> GetTeamDetailsAsync(Guid teamId, CancellationToken cancellationToken = default)
	{
		return await DbContext.Teams
			.AsNoTracking()
			.Where(team => team.Id == teamId)
			.Select(team => new TeamDetailsResult
			{
				Id = team.Id,
				ProjectId = team.ProjectId,
				Name = team.Name,
				Skills = team.Skills,
				FileUrl = team.FileUrl,
				CuratorId = team.CuratorId,
				CuratorFullName = team.Curator == null ? null : team.Curator.FullName,
				CreatedAt = team.CreatedAt,
				UpdatedAt = team.UpdatedAt,
				Project = team.Project == null
					? new TeamProjectDetailsResult()
					: new TeamProjectDetailsResult
					{
						Id = team.Project.Id,
						Title = team.Project.Title,
						Description = team.Project.Description,
						Goal = team.Project.Goal,
						Mvp = team.Project.Mvp,
						EvaluationCriteria = team.Project.Tasks,
						Deadline = team.Project.Deadline,
						Status = team.Project.Status,
						TeamsCount = team.Project.Teams.Count
					},
				Members = team.Members
					.OrderBy(member => member.StudentsProfile == null ? string.Empty : member.StudentsProfile.FullName)
					.Select(member => new TeamMemberResult
					{
						Id = member.Id,
						StudentProfileId = member.StudentsProfileId,
						FullName = member.StudentsProfile == null ? string.Empty : member.StudentsProfile.FullName,
						Email = member.StudentsProfile == null ? string.Empty : member.StudentsProfile.Email,
						RoleInTeam = member.StudentsProfile == null ? null : member.StudentsProfile.RoleInTeam
					})
					.ToList(),
				Iterations = team.Project == null
					? new List<TeamIterationResult>()
					: team.Project.Iterations
						.OrderBy(iteration => iteration.StartOn)
						.Select(iteration => new TeamIterationResult
						{
							Id = iteration.Id,
							Name = iteration.Name,
							StartOn = iteration.StartOn,
							EndOn = iteration.EndOn
						})
						.ToList()
			})
			.FirstOrDefaultAsync(cancellationToken);
	}

	private static IQueryable<Team> ApplyTeamListFilters(
		IQueryable<Team> query,
		string? search,
		TeamListFilter filter,
		Guid? projectId)
	{
		if (projectId.HasValue)
		{
			query = query.Where(team => team.ProjectId == projectId.Value);
		}

		if (!string.IsNullOrWhiteSpace(search))
		{
			var normalizedSearch = search.Trim().ToLower();
			query = query.Where(team =>
				team.Name.ToLower().Contains(normalizedSearch) ||
				(team.Skills != null && team.Skills.ToLower().Contains(normalizedSearch)) ||
				(team.Project != null && team.Project.Title.ToLower().Contains(normalizedSearch)) ||
				(team.Curator != null && team.Curator.FullName.ToLower().Contains(normalizedSearch)) ||
				team.Members.Any(member =>
					member.StudentsProfile != null &&
					member.StudentsProfile.FullName.ToLower().Contains(normalizedSearch)));
		}

		query = filter switch
		{
			TeamListFilter.ActiveOnProject => query.Where(team =>
				team.Project != null && team.Project.Status == ProjectStatus.Active),
			TeamListFilter.WithoutProject => query.Where(team => team.Project == null),
			_ => query
		};

		return query;
	}
}
