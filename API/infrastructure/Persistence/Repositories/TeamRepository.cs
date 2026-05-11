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

		var query = ApplyTeamListFilters(DbSet.AsNoTracking(), request.Search, request.Filter);
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

	private static IQueryable<Team> ApplyTeamListFilters(
		IQueryable<Team> query,
		string? search,
		TeamListFilter filter)
	{
		if (!string.IsNullOrWhiteSpace(search))
		{
			var normalizedSearch = search.Trim().ToLower();
			query = query.Where(team =>
				team.Name.ToLower().Contains(normalizedSearch) ||
				(team.Skills != null && team.Skills.ToLower().Contains(normalizedSearch)) ||
				(team.Project != null && team.Project.Title.ToLower().Contains(normalizedSearch)) ||
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
