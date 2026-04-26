using Entities.enums;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Projects;

public class CreateProjectRequest
{
    [Required]
    [MaxLength(250)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? ShortTitle { get; set; }

    [MaxLength(4000)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Goal { get; set; }

    [Required]
    [MaxLength(100)]
    public ProjectStatus Status { get; set; }

    [MaxLength(4000)]
    public string? Tasks { get; set; }

    [MaxLength(4000)]
    public string? Mvp { get; set; }
}
