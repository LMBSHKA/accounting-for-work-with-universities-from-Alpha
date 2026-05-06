using Application.Abstractions.Discussions;
using Application.Abstractions.Persistence;
using Application.Discussions.Models;
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
}
