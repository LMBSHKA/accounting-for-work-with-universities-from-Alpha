namespace Application.Discussions.Models;

public class CreateDiscussionCommentCommand
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string CommentBody { get; set; } = string.Empty;
}
