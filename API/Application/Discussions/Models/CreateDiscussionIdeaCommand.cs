using Entities.enums;

namespace Application.Discussions.Models;

public class CreateDiscussionIdeaCommand
{
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public ProjectStatus Status { get; set; } = ProjectStatus.Active;
	public Guid CreatedByUserId { get; set; }
}