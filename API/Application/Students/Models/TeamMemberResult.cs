namespace Application.Students.Models;

public class TeamMemberResult
{
    public Guid Id { get; set; }
    public Guid StudentProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? RoleInTeam { get; set; }
}
