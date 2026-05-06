namespace API.Contracts.Discussions;

public class GetDiscussionCommentsResponse
{
    public Guid ProjectId { get; set; }
    public IReadOnlyCollection<DiscussionCommentResponse> Items { get; set; } = [];
}
