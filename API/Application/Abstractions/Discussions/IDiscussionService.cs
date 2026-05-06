using Application.Discussions.Models;

namespace Application.Abstractions.Discussions;

public interface IDiscussionService
{
	Task<GetDiscussionIdeasResult> GetIdeasAsync(GetDiscussionIdeasQuery query, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<DiscussionCommentResult>?> GetCommentsAsync(Guid projectId, CancellationToken cancellationToken = default);
	Task<DiscussionCommentResult?> CreateCommentAsync(CreateDiscussionCommentCommand command, CancellationToken cancellationToken = default);
	Task<bool> SetProjectReactionAsync(SetProjectReactionCommand command, CancellationToken cancellationToken = default);
	Task<bool> DeleteProjectReactionAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
	Task<bool> SetProjectCommentReactionAsync(SetProjectCommentReactionCommand command, CancellationToken cancellationToken = default);
	Task<bool> DeleteProjectCommentReactionAsync(Guid projectCommentId, Guid userId, CancellationToken cancellationToken = default);
}
