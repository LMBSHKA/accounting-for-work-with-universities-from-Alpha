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
}
