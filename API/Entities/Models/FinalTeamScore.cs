namespace Entities.Models;

public class FinalTeamScore
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public Guid ScoredByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Team? Team { get; set; }
    public User? ScoredByUser { get; set; }
}
