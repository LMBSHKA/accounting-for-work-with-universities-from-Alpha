using API.Contracts.Discussions;
using Application.Abstractions.Discussions;
using Application.Discussions.Models;
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

		var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (!Guid.TryParse(userIdValue, out var userId))
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
			Replies = comment.Replies.Select(MapComment).ToList()
		};
	}
}
