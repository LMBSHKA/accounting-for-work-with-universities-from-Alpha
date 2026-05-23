using Entities.enums;

namespace Entities.Models;

public class Project
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? ShortTitle { get; set; }
	public string? Description { get; set; }
	public string? Goal { get; set; }
	public ProjectStatus Status { get; set; }
	public string? Tasks { get; set; }
	public string? Mvp { get; set; }
	public DateOnly? Deadline { get; set; }
	public Guid CreatedByUserId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public DateTime? ApprovedAt { get; set; }
	public DateTime? ArchivedAt { get; set; }

	public User? CreatedByUser { get; set; }
	public ICollection<ProjectStatusHistory> StatusHistory { get; set; } = new List<ProjectStatusHistory>();
	public ICollection<ProjectReaction> Reactions { get; set; } = new List<ProjectReaction>();
	public ICollection<ProjectComment> Comments { get; set; } = new List<ProjectComment>();
	public ICollection<Iteration> Iterations { get; set; } = new List<Iteration>();
	public ICollection<Team> Teams { get; set; } = new List<Team>();
	public ICollection<TeamResult> TeamResults { get; set; } = new List<TeamResult>();
}
