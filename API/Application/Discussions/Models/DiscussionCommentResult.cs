namespace Application.Discussions.Models;

public class DiscussionCommentResult
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public string? AuthorFullName { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string CommentBody { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DiscussionCommentResult> Replies { get; set; } = [];
}
