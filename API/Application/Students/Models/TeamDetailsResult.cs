using Entities.enums;

namespace Application.Students.Models;

public class TeamDetailsResult
{
	public Guid Id { get; set; }
	public Guid ProjectId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Skills { get; set; }
	public string? FileUrl { get; set; }
	public Guid? CuratorId { get; set; }
	public string? CuratorFullName { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public TeamProjectDetailsResult Project { get; set; } = new();
	public List<TeamMemberResult> Members { get; set; } = [];
	public List<TeamIterationResult> Iterations { get; set; } = [];
}

public class TeamProjectDetailsResult
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

public class TeamIterationResult
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateOnly StartOn { get; set; }
	public DateOnly EndOn { get; set; }
}
