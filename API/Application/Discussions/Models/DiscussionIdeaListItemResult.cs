using Entities.enums;

namespace Application.Discussions.Models;

public class DiscussionIdeaListItemResult
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public ProjectStatus Status { get; set; }
	public string? AuthorFullName { get; set; }
	public int LikeReactionsCount { get; set; }
	public int DislikeReactionsCount { get; set; }
}