using Entities.enums;

namespace Application.Projects.Models;

public class UpdateProjectStatusCommand
{
	public Guid ProjectId { get; set; }
	public ProjectStatus Status { get; set; }
	public Guid ChangedByUserId { get; set; }
}
