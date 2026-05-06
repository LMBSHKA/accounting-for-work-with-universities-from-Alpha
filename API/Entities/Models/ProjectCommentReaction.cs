using Entities.enums;

namespace Entities.Models;

public class ProjectCommentReaction
{
    public Guid Id { get; set; }
    public Guid ProjectCommentId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }
    public DateTime CreatedAt { get; set; }

    public ProjectComment? ProjectComment { get; set; }
    public User? User { get; set; }
}
