namespace Entities.Models;

public class IterationMemberScore
{
    public Guid Id { get; set; }
    public Guid IterationId { get; set; }
    public Guid MemberId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public Guid ScoredByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Iteration? Iteration { get; set; }
    public TeamMember? Member { get; set; }
    public User? ScoredByUser { get; set; }
}
