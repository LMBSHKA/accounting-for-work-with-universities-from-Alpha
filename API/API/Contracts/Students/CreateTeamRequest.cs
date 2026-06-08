using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Students;

public class CreateTeamRequest
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Skills { get; set; }

    [MaxLength(1024)]
    public string? FileUrl { get; set; }

    public Guid? CuratorId { get; set; }

    [Required]
    [MinLength(1)]
    public List<Guid> StudentProfileIds { get; set; } = [];
}
