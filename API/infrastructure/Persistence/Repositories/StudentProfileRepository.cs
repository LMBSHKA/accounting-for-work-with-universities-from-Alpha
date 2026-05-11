using Application.Abstractions.Persistence;
using Application.Students.Models;
using Entities.enums;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class StudentProfileRepository(AppDbContext dbContext) : Repository<StudentProfile>(dbContext), IStudentProfileRepository
{
	public Task<StudentProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var normalizedEmail = email.Trim().ToLower();

		return DbSet.FirstOrDefaultAsync(student =>
			student.Email.ToLower() == normalizedEmail,
			cancellationToken);
	}

	public async Task<GetStudentsResult> GetStudentsAsync(GetStudentsQuery request, CancellationToken cancellationToken = default)
	{
		var offset = Math.Max(0, request.Offset);
		var limit = Math.Clamp(request.Limit, 1, 100);

		var query = ApplyStudentListFilters(DbSet.AsNoTracking(), request.Search, request.Filter);
		var totalCount = await query.CountAsync(cancellationToken);

		var students = await query
			.OrderBy(student => student.FullName)
			.ThenBy(student => student.Email)
			.Skip(offset)
			.Take(limit)
			.Select(student => new StudentListItemResult
			{
				Id = student.Id,
				FullName = student.FullName,
				Email = student.Email,
				RoleInTeam = student.RoleInTeam,
				UpdatedAt = student.UpdatedAt,
				TeamNames = student.TeamMemberships
					.Where(member => member.Team != null)
					.Select(member => member.Team!.Name)
					.Distinct()
					.OrderBy(name => name)
					.ToList()
			})
			.ToListAsync(cancellationToken);

		var loadedCount = offset + students.Count;

		return new GetStudentsResult
		{
			Items = students,
			TotalCount = totalCount,
			Offset = offset,
			Limit = limit,
			LoadedCount = loadedCount,
			HasMore = loadedCount < totalCount,
			NextOffset = loadedCount < totalCount ? loadedCount : null
		};
	}

	private static IQueryable<StudentProfile> ApplyStudentListFilters(
		IQueryable<StudentProfile> query,
		string? search,
		StudentListFilter filter)
	{
		if (!string.IsNullOrWhiteSpace(search))
		{
			var normalizedSearch = search.Trim().ToLower();
			query = query.Where(student =>
				student.FullName.ToLower().Contains(normalizedSearch) ||
				student.Email.ToLower().Contains(normalizedSearch) ||
				student.TeamMemberships.Any(member =>
					member.Team != null && member.Team.Name.ToLower().Contains(normalizedSearch)));
		}

		query = filter switch
		{
			StudentListFilter.ActiveOnProject => query.Where(student =>
				student.TeamMemberships.Any(member =>
					member.Team != null &&
					member.Team.Project != null &&
					member.Team.Project.Status == ProjectStatus.Active)),
			StudentListFilter.WithoutProject => query.Where(student => !student.TeamMemberships.Any()),
			_ => query
		};

		return query;
	}
}
