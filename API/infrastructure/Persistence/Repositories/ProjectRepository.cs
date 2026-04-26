using Application.Abstractions.Persistence;
using Application.Projects.Models;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProjectRepository(AppDbContext dbContext) : Repository<Project>(dbContext), IProjectRepository
{
    public async Task<GetProjectsResult> GetProjectsAsync(GetProjectsQuery request, CancellationToken cancellationToken = default)
    {
        var offset = Math.Max(0, request.Offset);
        var limit = Math.Clamp(request.Limit, 1, 100);

        var query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(project => project.Title.ToLower().Contains(search));
        }

        if (request.Statuses is { Count: > 0 })
        {
            query = query.Where(project => request.Statuses.Contains(project.Status));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var projects = await query
            .OrderByDescending(project => project.CreatedAt)
            .ThenBy(project => project.Title)
            .Skip(offset)
            .Take(limit)
            .Select(project => new ProjectListItemResult
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                Status = project.Status,
                TeamsCount = project.Teams.Count,
            })
            .ToListAsync(cancellationToken);

        var loadedCount = offset + projects.Count;

        return new GetProjectsResult
        {
            Items = projects,
            TotalCount = totalCount,
            Offset = offset,
            Limit = limit,
            LoadedCount = loadedCount,
            HasMore = loadedCount < totalCount,
            NextOffset = loadedCount < totalCount ? loadedCount : null
        };
    }
}
