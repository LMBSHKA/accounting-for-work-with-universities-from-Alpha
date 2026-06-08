namespace Application.Students.Models;

public class CuratorListItemResult
{
	public Guid Id { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string SystemRole { get; set; } = string.Empty;
	public List<CuratorTeamResult> Teams { get; set; } = [];
}

public class CuratorTeamResult
{
	public Guid Id { get; set; }
	public Guid ProjectId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? ProjectTitle { get; set; }
}
