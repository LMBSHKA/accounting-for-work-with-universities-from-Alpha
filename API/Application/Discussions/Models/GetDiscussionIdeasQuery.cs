using Entities.enums;

namespace Application.Discussions.Models;

public class GetDiscussionIdeasQuery
{
    public string? Search { get; set; }
    public List<ProjectStatus>? Statuses { get; set; }
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 8;
}
