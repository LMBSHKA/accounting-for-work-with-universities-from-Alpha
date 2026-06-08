namespace API.Contracts.Students;

public class TeamResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Skills { get; set; }
    public string? FileUrl { get; set; }
    public Guid? CuratorId { get; set; }
    public string? CuratorFullName { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<TeamMemberResponse> Members { get; set; } = [];
}
