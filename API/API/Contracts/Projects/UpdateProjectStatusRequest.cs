using Entities.enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Projects;

public class UpdateProjectStatusRequest
{
	[Description("""
        Новый статус проекта.

        Значения:
        1 — Active: активный проект.
        2 — Rejected: отклоненный проект.
        3 — Archived: архивный проект.
        4 — Completed: завершенный проект.
        5 — Idea: идея.
     """)]
	[Required]
	public ProjectStatus Status { get; set; }

}
