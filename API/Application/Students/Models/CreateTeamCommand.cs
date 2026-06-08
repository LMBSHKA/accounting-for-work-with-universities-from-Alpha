namespace Application.Students.Models;

public class CreateTeamCommand
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Skills { get; set; }
    public string? FileUrl { get; set; }
    public Guid? CuratorId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public List<Guid> StudentProfileIds { get; set; } = [];
}
