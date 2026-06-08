namespace API.Contracts.Students;

public class CuratorListItemResponse
{
	public Guid Id { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string SystemRole { get; set; } = string.Empty;
	public List<CuratorTeamResponse> Teams { get; set; } = [];
}

public class CuratorTeamResponse
{
	public Guid Id { get; set; }
	public Guid ProjectId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? ProjectTitle { get; set; }
}
