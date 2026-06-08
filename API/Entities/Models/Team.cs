namespace Entities.Models;

public class Team
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Skills { get; set; }
    public string? FileUrl { get; set; }
    public Guid? CuratorId { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Project? Project { get; set; }
    public User? CreatedByUser { get; set; }
    public User? Curator { get; set; }
    public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
    public ICollection<IterationTeamScore> IterationScores { get; set; } = new List<IterationTeamScore>();
    public ICollection<FinalTeamScore> FinalScores { get; set; } = new List<FinalTeamScore>();
    public ICollection<TeamResult> Results { get; set; } = new List<TeamResult>();
}
