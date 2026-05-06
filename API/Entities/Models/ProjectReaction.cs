using Entities.enums;

namespace Entities.Models;

public class ProjectReaction
{
	public Guid Id { get; set; }
	public Guid ProjectId { get; set; }
	public string? Description { get; set; }
	public ReactionType ReactionType { get; set; }
	public Guid CreatedByUserId { get; set; }
	public DateTime CreatedAt { get; set; }

	public Project? Project { get; set; }
	public User? CreatedByUser { get; set; }
}
