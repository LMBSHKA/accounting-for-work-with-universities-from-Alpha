using Entities.enums;

namespace API.Contracts.Projects;

public class ProjectListItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public int TeamsCount { get; set; }
}
