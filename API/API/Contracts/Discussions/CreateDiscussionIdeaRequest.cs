using Entities.enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Discussions;

public class CreateDiscussionIdeaRequest
{
	[Description("Название идеи.")]
	[Required]
	[MaxLength(250)]
	public string Title { get; set; } = string.Empty;

	[Description("Описание идеи.")]
	[MaxLength(4000)]
	public string? Description { get; set; }

	[Description("""
    Статус идеи.
    Если не передавать, будет установлен статус 1 — Active.

    Значения:
    1 — Active: активная идея.
    2 — Rejected: отклоненная идея.
    3 — Archived: архивная идея.
    4 — Completed: завершенная идея.
    """)]
	public ProjectStatus? Status { get; set; } = ProjectStatus.Active;
}