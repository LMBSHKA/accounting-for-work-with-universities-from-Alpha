using Entities.enums;

namespace Application.Discussions.Models;

public class SetProjectCommentReactionCommand
{
    public Guid ProjectCommentId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }
}
