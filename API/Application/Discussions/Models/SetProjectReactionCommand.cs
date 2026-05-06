using Entities.enums;

namespace Application.Discussions.Models;

public class SetProjectReactionCommand
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }
}
