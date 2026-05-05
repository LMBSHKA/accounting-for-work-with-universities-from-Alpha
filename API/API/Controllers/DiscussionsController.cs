using API.Contracts.Discussions;
using Application.Abstractions.Discussions;
using Application.Discussions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}
