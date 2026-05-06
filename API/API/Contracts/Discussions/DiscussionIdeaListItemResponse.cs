using Entities.enums;

namespace API.Contracts.Discussions;

public class DiscussionIdeaListItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public string? AuthorFullName { get; set; }
    public int LikeReactionsCount { get; set; }
    public int DislikeReactionsCount { get; set; }
}
