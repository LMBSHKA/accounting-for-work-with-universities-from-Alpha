using Application.Abstractions.Discussions;
using Application.Abstractions.Persistence;
using Application.Discussions.Models;
using Entities.enums;
using Entities.Models;

namespace Application.Discussions.Services;

public class DiscussionService(IUnitOfWork unitOfWork) : IDiscussionService
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public Task<GetDiscussionIdeasResult> GetIdeasAsync(GetDiscussionIdeasQuery query, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.Projects.GetDiscussionIdeasAsync(query, cancellationToken);
	}

	public async Task<IReadOnlyCollection<DiscussionCommentResult>?> GetCommentsAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
		if (project is null)
		{
			return null;
		}

		var comments = await _unitOfWork.Projects.GetDiscussionCommentsAsync(projectId, cancellationToken);
		return BuildCommentTree(comments);
	}

	public async Task<DiscussionCommentResult?> CreateCommentAsync(CreateDiscussionCommentCommand command, CancellationToken cancellationToken = default)
	{
		var commentBody = command.CommentBody.Trim();
		if (string.IsNullOrWhiteSpace(commentBody))
		{
			return null;
		}

		var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken);
		if (project is null)
		{
			return null;
		}

		if (command.ParentCommentId.HasValue)
		{
			var parentComment = await _unitOfWork.ProjectComments.GetByIdAsync(command.ParentCommentId.Value, cancellationToken);
			if (parentComment is null || parentComment.ProjectId != command.ProjectId)
			{
				return null;
			}
		}

		var comment = new ProjectComment
		{
			Id = Guid.NewGuid(),
			ProjectId = command.ProjectId,
			UserId = command.UserId,
			ParentCommentId = command.ParentCommentId,
			CommentBody = commentBody,
			CreatedAt = DateTime.UtcNow
		};

		await _unitOfWork.ProjectComments.AddAsync(comment, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		var user = await _unitOfWork.Users.GetByIdAsync(command.UserId, cancellationToken);

		return new DiscussionCommentResult
		{
			Id = comment.Id,
			ProjectId = comment.ProjectId,
			UserId = comment.UserId,
			AuthorFullName = user?.FullName,
			ParentCommentId = comment.ParentCommentId,
			CommentBody = comment.CommentBody,
			CreatedAt = comment.CreatedAt,
			UpdatedAt = comment.UpdatedAt
		};
	}

	public async Task<DiscussionIdeaListItemResult?> CreateIdeaAsync(
	CreateDiscussionIdeaCommand command,
	CancellationToken cancellationToken = default)
	{
		var title = NormalizeRequired(command.Title);
		if (string.IsNullOrWhiteSpace(title) || command.CreatedByUserId == Guid.Empty)
		{
			return null;
		}

		var user = await _unitOfWork.Users.GetByIdAsync(command.CreatedByUserId, cancellationToken);
		if (user is null)
		{
			return null;
		}

		var now = DateTime.UtcNow;
		const ProjectStatus ideaStatus = ProjectStatus.Idea;

		var project = new Project
		{
			Id = Guid.NewGuid(),
			Title = title,
			Description = NormalizeOptional(command.Description),
			Status = ideaStatus,
			CreatedByUserId = command.CreatedByUserId,
			CreatedAt = now
		};

		await _unitOfWork.Projects.AddAsync(project, cancellationToken);

		await _unitOfWork.ProjectStatusHistory.AddAsync(new ProjectStatusHistory
		{
			Id = Guid.NewGuid(),
			ProjectId = project.Id,
			Status = project.Status,
			ChangedByUserId = command.CreatedByUserId,
			ChangeComment = "Идея создана",
			ChangedAt = now
		}, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return new DiscussionIdeaListItemResult
		{
			Id = project.Id,
			Title = project.Title,
			Description = project.Description,
			Status = project.Status,
			AuthorFullName = user.FullName,
			LikeReactionsCount = 0,
			DislikeReactionsCount = 0
		};
	}

	public async Task<bool> SetProjectReactionAsync(SetProjectReactionCommand command, CancellationToken cancellationToken = default)
	{
		var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken);
		if (project is null)
		{
			return false;
		}

		var reaction = await _unitOfWork.Projects.GetProjectReactionAsync(command.ProjectId, command.UserId, cancellationToken);
		if (reaction is null)
		{
			reaction = new ProjectReaction
			{
				Id = Guid.NewGuid(),
				ProjectId = command.ProjectId,
				CreatedByUserId = command.UserId,
				ReactionType = command.ReactionType,
				CreatedAt = DateTime.UtcNow
			};

			await _unitOfWork.ProjectReactions.AddAsync(reaction, cancellationToken);
		}
		else if (reaction.ReactionType != command.ReactionType)
		{
			reaction.ReactionType = command.ReactionType;
			_unitOfWork.ProjectReactions.Update(reaction);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteProjectReactionAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
	{
		var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
		if (project is null)
		{
			return false;
		}

		var reaction = await _unitOfWork.Projects.GetProjectReactionAsync(projectId, userId, cancellationToken);
		if (reaction is not null)
		{
			_unitOfWork.ProjectReactions.Remove(reaction);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		return true;
	}

	public async Task<bool> SetProjectCommentReactionAsync(SetProjectCommentReactionCommand command, CancellationToken cancellationToken = default)
	{
		var comment = await _unitOfWork.ProjectComments.GetByIdAsync(command.ProjectCommentId, cancellationToken);
		if (comment is null)
		{
			return false;
		}

		var reaction = await _unitOfWork.Projects.GetProjectCommentReactionAsync(command.ProjectCommentId, command.UserId, cancellationToken);
		if (reaction is null)
		{
			reaction = new ProjectCommentReaction
			{
				Id = Guid.NewGuid(),
				ProjectCommentId = command.ProjectCommentId,
				UserId = command.UserId,
				ReactionType = command.ReactionType,
				CreatedAt = DateTime.UtcNow
			};

			await _unitOfWork.ProjectCommentReactions.AddAsync(reaction, cancellationToken);
		}
		else if (reaction.ReactionType != command.ReactionType)
		{
			reaction.ReactionType = command.ReactionType;
			_unitOfWork.ProjectCommentReactions.Update(reaction);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteProjectCommentReactionAsync(Guid projectCommentId, Guid userId, CancellationToken cancellationToken = default)
	{
		var comment = await _unitOfWork.ProjectComments.GetByIdAsync(projectCommentId, cancellationToken);
		if (comment is null)
		{
			return false;
		}

		var reaction = await _unitOfWork.Projects.GetProjectCommentReactionAsync(projectCommentId, userId, cancellationToken);
		if (reaction is not null)
		{
			_unitOfWork.ProjectCommentReactions.Remove(reaction);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		return true;
	}

	private static IReadOnlyCollection<DiscussionCommentResult> BuildCommentTree(IReadOnlyCollection<DiscussionCommentResult> comments)
	{
		var map = comments.ToDictionary(comment => comment.Id);
		var roots = new List<DiscussionCommentResult>();

		foreach (var comment in comments.OrderBy(comment => comment.CreatedAt))
		{
			comment.Replies = [];
		}

		foreach (var comment in comments.OrderBy(comment => comment.CreatedAt))
		{
			if (comment.ParentCommentId.HasValue && map.TryGetValue(comment.ParentCommentId.Value, out var parent))
			{
				parent.Replies.Add(comment);
			}
			else
			{
				roots.Add(comment);
			}
		}

		return roots;
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
