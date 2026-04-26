using Entities.enums;

namespace Application.Projects.Models;

public class GetProjectsQuery
{
    public string? Search { get; set; }
    public List<ProjectStatus>? Statuses { get; set; }
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 8;
}
