namespace Entities.Models;

public class TeamMember
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid StudentsProfileId { get; set; }

    public Team? Team { get; set; }
    public StudentProfile? StudentsProfile { get; set; }
    public ICollection<IterationMemberScore> IterationScores { get; set; } = new List<IterationMemberScore>();
}
