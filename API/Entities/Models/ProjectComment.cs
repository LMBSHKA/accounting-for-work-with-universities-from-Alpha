namespace Entities.Models;

public class ProjectComment
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string CommentBody { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Project? Project { get; set; }
    public User? User { get; set; }
    public ProjectComment? ParentComment { get; set; }
    public ICollection<ProjectComment> Replies { get; set; } = new List<ProjectComment>();
}
