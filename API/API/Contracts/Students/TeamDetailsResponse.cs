using Entities.enums;

namespace API.Contracts.Students;

public class TeamDetailsResponse
{
	public Guid Id { get; set; }
	public Guid ProjectId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Skills { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public TeamProjectDetailsResponse Project { get; set; } = new();
	public List<TeamMemberResponse> Members { get; set; } = [];
	public List<TeamIterationResponse> Iterations { get; set; } = [];
}

public class TeamProjectDetailsResponse
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? Goal { get; set; }
	public string? Mvp { get; set; }
	public string? EvaluationCriteria { get; set; }
	public DateOnly? Deadline { get; set; }
	public ProjectStatus Status { get; set; }
	public int TeamsCount { get; set; }
}

public class TeamIterationResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateOnly StartOn { get; set; }
	public DateOnly EndOn { get; set; }
}
