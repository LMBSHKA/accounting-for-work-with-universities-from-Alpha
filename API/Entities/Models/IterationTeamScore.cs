namespace Entities.Models;

public class IterationTeamScore
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid IterationId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public Guid ScoredByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Team? Team { get; set; }
    public Iteration? Iteration { get; set; }
    public User? ScoredByUser { get; set; }
}
