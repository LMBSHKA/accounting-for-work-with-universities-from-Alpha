using Application.Discussions.Models;

namespace Application.Abstractions.Discussions;

public interface IDiscussionService
{
	Task<GetDiscussionIdeasResult> GetIdeasAsync(GetDiscussionIdeasQuery query, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<DiscussionCommentResult>?> GetCommentsAsync(Guid projectId, CancellationToken cancellationToken = default);
	Task<DiscussionCommentResult?> CreateCommentAsync(CreateDiscussionCommentCommand command, CancellationToken cancellationToken = default);
}
