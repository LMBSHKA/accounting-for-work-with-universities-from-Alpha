using Entities.enums;

namespace Application.Projects.Models;

public class ProjectListItemResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public int TeamsCount { get; set; }
}
