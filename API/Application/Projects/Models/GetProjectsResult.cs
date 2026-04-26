namespace Application.Projects.Models;

public class GetProjectsResult
{
    public IReadOnlyCollection<ProjectListItemResult> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    public int LoadedCount { get; set; }
    public bool HasMore { get; set; }
    public int? NextOffset { get; set; }
}
