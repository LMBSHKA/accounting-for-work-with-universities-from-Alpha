using Application.Discussions.Models;

namespace Application.Abstractions.Discussions;

public interface IDiscussionService
{
    Task<GetDiscussionIdeasResult> GetIdeasAsync(GetDiscussionIdeasQuery query, CancellationToken cancellationToken = default);
}
