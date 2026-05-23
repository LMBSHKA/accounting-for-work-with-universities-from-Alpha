using Entities.enums;

namespace Application.Projects.Models;

public class ProjectStatusHistoryResult
{
	public Guid Id { get; set; }
	public Guid ProjectId { get; set; }
	public ProjectStatus Status { get; set; }
	public Guid ChangedByUserId { get; set; }
	public string? ChangedByUserFullName { get; set; }
	public string? ChangeComment { get; set; }
	public DateTime ChangedAt { get; set; }
}
