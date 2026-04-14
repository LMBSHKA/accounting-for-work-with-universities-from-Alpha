namespace Entities.Models;

public class StudentProfile
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? RoleInTeam { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();
    public ICollection<FinalMemberScore> FinalScores { get; set; } = new List<FinalMemberScore>();
}
