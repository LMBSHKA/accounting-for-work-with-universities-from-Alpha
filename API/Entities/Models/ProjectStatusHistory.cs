using Entities.enums;

namespace Entities.Models;

public class ProjectStatusHistory
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectStatus Status { get; set; }
    public Guid ChangedByUserId { get; set; }
    public string? ChangeComment { get; set; }
    public DateTime ChangedAt { get; set; }

    public Project? Project { get; set; }
    public User? ChangedByUser { get; set; }
}
