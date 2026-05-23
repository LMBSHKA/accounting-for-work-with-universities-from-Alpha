namespace Entities.Models;

public class Iteration
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartOn { get; set; }
    public DateOnly EndOn { get; set; }
    public string? EvaluationCriteria { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Project? Project { get; set; }
    public ICollection<IterationTeamScore> TeamScores { get; set; } = new List<IterationTeamScore>();
    public ICollection<IterationMemberScore> MemberScores { get; set; } = new List<IterationMemberScore>();
    public ICollection<FinalMemberScore> FinalMemberScores { get; set; } = new List<FinalMemberScore>();
}
