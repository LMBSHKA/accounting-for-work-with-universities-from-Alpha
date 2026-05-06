using Application.Abstractions.Persistence;
using Application.Discussions.Models;
using Application.Projects.Models;
using Entities.enums;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProjectRepository(AppDbContext dbContext) : Repository<Project>(dbContext), IProjectRepository
{
	public async Task<GetProjectsResult> GetProjectsAsync(GetProjectsQuery request, CancellationToken cancellationToken = default)
	{
		var offset = Math.Max(0, request.Offset);
		var limit = Math.Clamp(request.Limit, 1, 100);

		var query = ApplyProjectListFilters(DbSet.AsNoTracking(), request.Search, request.Statuses);
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

	public async Task<GetDiscussionIdeasResult> GetDiscussionIdeasAsync(GetDiscussionIdeasQuery request, CancellationToken cancellationToken = default)
	{
		var offset = Math.Max(0, request.Offset);
		var limit = Math.Clamp(request.Limit, 1, 100);

		var query = ApplyProjectListFilters(DbSet.AsNoTracking(), request.Search, request.Statuses);
		var totalCount = await query.CountAsync(cancellationToken);

		var ideas = await query
			.OrderByDescending(project => project.CreatedAt)
			.ThenBy(project => project.Title)
			.Skip(offset)
			.Take(limit)
			.Select(project => new DiscussionIdeaListItemResult
			{
				Id = project.Id,
				Title = project.Title,
				Status = project.Status,
				AuthorFullName = project.CreatedByUser == null ? null : project.CreatedByUser.FullName,
				LikeReactionsCount = project.Reactions.Count(reaction => reaction.ReactionType == ReactionType.Like),
				DislikeReactionsCount = project.Reactions.Count(reaction => reaction.ReactionType == ReactionType.Dislike),
			})
			.ToListAsync(cancellationToken);

		var loadedCount = offset + ideas.Count;

		return new GetDiscussionIdeasResult
		{
			Items = ideas,
			TotalCount = totalCount,
			Offset = offset,
			Limit = limit,
			LoadedCount = loadedCount,
			HasMore = loadedCount < totalCount,
			NextOffset = loadedCount < totalCount ? loadedCount : null
		};
	}

	public async Task<IReadOnlyCollection<DiscussionCommentResult>> GetDiscussionCommentsAsync(
	Guid projectId,
	CancellationToken cancellationToken = default)
	{
		return await DbContext.ProjectComments
			.AsNoTracking()
			.Where(comment => comment.ProjectId == projectId)
			.OrderBy(comment => comment.CreatedAt)
			.Select(comment => new DiscussionCommentResult
			{
				Id = comment.Id,
				ProjectId = comment.ProjectId,
				UserId = comment.UserId,
				AuthorFullName = comment.User == null ? null : comment.User.FullName,
				ParentCommentId = comment.ParentCommentId,
				CommentBody = comment.CommentBody,
				CreatedAt = comment.CreatedAt,
				UpdatedAt = comment.UpdatedAt,
				LikeReactionsCount = comment.Reactions.Count(reaction => reaction.ReactionType == ReactionType.Like),
				DislikeReactionsCount = comment.Reactions.Count(reaction => reaction.ReactionType == ReactionType.Dislike)
			})
			.ToListAsync(cancellationToken);
	}


	public async Task<ProjectReaction?> GetProjectReactionAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
	{
		return await DbContext.ProjectReactions
			.FirstOrDefaultAsync(reaction =>
				reaction.ProjectId == projectId &&
				reaction.CreatedByUserId == userId, cancellationToken);
	}

	public async Task<ProjectCommentReaction?> GetProjectCommentReactionAsync(Guid projectCommentId, Guid userId, CancellationToken cancellationToken = default)
	{
		return await DbContext.ProjectCommentReactions
			.FirstOrDefaultAsync(reaction =>
				reaction.ProjectCommentId == projectCommentId &&
				reaction.UserId == userId, cancellationToken);
	}

	private static IQueryable<Project> ApplyProjectListFilters(
		IQueryable<Project> query,
		string? search,
		List<Entities.enums.ProjectStatus>? statuses)
	{
		if (!string.IsNullOrWhiteSpace(search))
		{
			var normalizedSearch = search.Trim().ToLower();
			query = query.Where(project => project.Title.ToLower().Contains(normalizedSearch));
		}

		if (statuses is { Count: > 0 })
		{
			query = query.Where(project => statuses.Contains(project.Status));
		}

		return query;
	}
}
