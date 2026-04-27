using Entities.enums;
using System.ComponentModel;
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

	[Description("""
        Статус проекта.

        Значения:
        1 — Active: активный проект.
        2 — Rejected: отклоненный проект.
        3 — Archived: архивный проект.
        4 — Completed: завершенный проект.
     """)]
	[Required]
    public ProjectStatus Status { get; set; }

    [MaxLength(4000)]
    public string? Tasks { get; set; }

    [MaxLength(4000)]
    public string? Mvp { get; set; }
}
