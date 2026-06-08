using API.Contracts.Discussions;
using Application.Abstractions.Discussions;
using Application.Discussions.Models;
using Entities.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/discussion")]
[Authorize]
public class DiscussionsController(IDiscussionService discussionService) : ControllerBase
{
	private readonly IDiscussionService _discussionService = discussionService;

	[EndpointSummary("Список идей для страницы обсуждения")]
	[HttpPost("list")]
	public async Task<ActionResult<GetDiscussionIdeasResponse>> GetIdeas(
		[FromBody] GetDiscussionIdeasRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var result = await _discussionService.GetIdeasAsync(new GetDiscussionIdeasQuery
		{
			Search = request.Search,
			Statuses = request.Statuses,
			Offset = request.Offset,
			Limit = request.Limit
		}, cancellationToken);

		return Ok(new GetDiscussionIdeasResponse
		{
			Items = result.Items.Select(idea => new DiscussionIdeaListItemResponse
			{
				Id = idea.Id,
				Title = idea.Title,
				Status = idea.Status,
				AuthorFullName = idea.AuthorFullName,
				LikeReactionsCount = idea.LikeReactionsCount,
				DislikeReactionsCount = idea.DislikeReactionsCount,
			}).ToList(),
			TotalCount = result.TotalCount,
			Offset = result.Offset,
			Limit = result.Limit,
			LoadedCount = result.LoadedCount,
			HasMore = result.HasMore,
			NextOffset = result.NextOffset
		});
	}

	[EndpointSummary("Список комментариев для определенной идеи")]
	[HttpGet("{ideaId:guid}/comments")]
	public async Task<ActionResult<GetDiscussionCommentsResponse>> GetComments(
		Guid ideaId,
		CancellationToken cancellationToken)
	{
		var comments = await _discussionService.GetCommentsAsync(ideaId, cancellationToken);
		if (comments is null)
		{
			return NotFound(new { message = "Idea was not found." });
		}

		return Ok(new GetDiscussionCommentsResponse
		{
			ProjectId = ideaId,
			Items = comments.Select(MapComment).ToList()
		});
	}

	[EndpointSummary("Добавление комментария к идее")]
	[HttpPost("{ideaId:guid}/comments")]
	public async Task<ActionResult<DiscussionCommentResponse>> CreateComment(
		Guid ideaId,
		[FromBody] CreateDiscussionCommentRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		if (!TryGetCurrentUserId(out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		var comment = await _discussionService.CreateCommentAsync(new CreateDiscussionCommentCommand
		{
			ProjectId = ideaId,
			UserId = userId,
			ParentCommentId = request.ParentCommentId,
			CommentBody = request.CommentBody
		}, cancellationToken);

		if (comment is null)
		{
			return BadRequest(new { message = "Idea was not found, parent comment is invalid, or comment body is empty." });
		}

		return Ok(MapComment(comment));
	}


	[EndpointSummary("Установка реакции текущего пользователя на идею")]
	[HttpPost("{projectId:guid}/reaction")]
	public async Task<IActionResult> SetProjectReaction(
		Guid projectId,
		[FromBody] SetDiscussionReactionRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		if (!TryGetCurrentUserId(out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		if (!Enum.IsDefined(typeof(ReactionType), request.ReactionType))
		{
			return BadRequest(new { message = "ReactionType must be 1 (Like) or 2 (Dislike)." });
		}

		var isSuccess = await _discussionService.SetProjectReactionAsync(new SetProjectReactionCommand
		{
			ProjectId = projectId,
			UserId = userId,
			ReactionType = (ReactionType)request.ReactionType
		}, cancellationToken);

		if (!isSuccess)
		{
			return NotFound(new { message = "Idea was not found." });
		}

		return NoContent();
	}

	[EndpointSummary("Удаление реакции текущего пользователя с идеи")]
	[HttpDelete("{projectId:guid}/reaction")]
	public async Task<IActionResult> DeleteProjectReaction(
		Guid projectId,
		CancellationToken cancellationToken)
	{
		if (!TryGetCurrentUserId(out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		var isSuccess = await _discussionService.DeleteProjectReactionAsync(projectId, userId, cancellationToken);
		if (!isSuccess)
		{
			return NotFound(new { message = "Idea was not found." });
		}

		return NoContent();
	}

	[EndpointSummary("Установка реакции текущего пользователя на комментарий")]
	[HttpPost("comments/{commentId:guid}/reaction")]
	public async Task<IActionResult> SetCommentReaction(
		Guid commentId,
		[FromBody] SetDiscussionReactionRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		if (!TryGetCurrentUserId(out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		if (!Enum.IsDefined(typeof(ReactionType), request.ReactionType))
		{
			return BadRequest(new { message = "ReactionType must be 1 (Like) or 2 (Dislike)." });
		}

		var isSuccess = await _discussionService.SetProjectCommentReactionAsync(new SetProjectCommentReactionCommand
		{
			ProjectCommentId = commentId,
			UserId = userId,
			ReactionType = (ReactionType)request.ReactionType
		}, cancellationToken);

		if (!isSuccess)
		{
			return NotFound(new { message = "Comment was not found." });
		}

		return NoContent();
	}

	[EndpointSummary("Удаление реакции текущего пользователя с комментария")]
	[HttpDelete("comments/{commentId:guid}/reaction")]
	public async Task<IActionResult> DeleteCommentReaction(
		Guid commentId,
		CancellationToken cancellationToken)
	{
		if (!TryGetCurrentUserId(out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		var isSuccess = await _discussionService.DeleteProjectCommentReactionAsync(commentId, userId, cancellationToken);
		if (!isSuccess)
		{
			return NotFound(new { message = "Comment was not found." });
		}

		return NoContent();
	}

	[EndpointSummary("Добавление идеи для страницы обсуждения")]
	[HttpPost]
	public async Task<ActionResult<DiscussionIdeaListItemResponse>> CreateIdea(
	[FromBody] CreateDiscussionIdeaRequest request,
	CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		if (!TryGetCurrentUserId(out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		var idea = await _discussionService.CreateIdeaAsync(new CreateDiscussionIdeaCommand
		{
			Title = request.Title,
			Description = request.Description,
			Status = ProjectStatus.Idea,
			CreatedByUserId = userId
		}, cancellationToken);

		if (idea is null)
		{
			return BadRequest(new { message = "Idea title is empty or author was not found." });
		}

		return Ok(MapIdea(idea));
	}

	private bool TryGetCurrentUserId(out Guid userId)
	{
		var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
		return Guid.TryParse(userIdValue, out userId);
	}

	private static DiscussionCommentResponse MapComment(DiscussionCommentResult comment)
	{
		return new DiscussionCommentResponse
		{
			Id = comment.Id,
			ProjectId = comment.ProjectId,
			UserId = comment.UserId,
			AuthorFullName = comment.AuthorFullName,
			ParentCommentId = comment.ParentCommentId,
			CommentBody = comment.CommentBody,
			CreatedAt = comment.CreatedAt,
			UpdatedAt = comment.UpdatedAt,
			LikeReactionsCount = comment.LikeReactionsCount,
			DislikeReactionsCount = comment.DislikeReactionsCount,
			Replies = comment.Replies.Select(MapComment).ToList()
		};
	}

	private static DiscussionIdeaListItemResponse MapIdea(DiscussionIdeaListItemResult idea)
	{
		return new DiscussionIdeaListItemResponse
		{
			Id = idea.Id,
			Title = idea.Title,
			Description = idea.Description,
			Status = idea.Status,
			AuthorFullName = idea.AuthorFullName,
			LikeReactionsCount = idea.LikeReactionsCount,
			DislikeReactionsCount = idea.DislikeReactionsCount
		};
	}
}
