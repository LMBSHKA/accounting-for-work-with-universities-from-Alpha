namespace Application.Students.Models;

public class CreateStudentCommand
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? RoleInTeam { get; set; }
}
