using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Students;

public class CreateStudentRequest
{
    [Required]
    [MaxLength(256)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? RoleInTeam { get; set; }
}
