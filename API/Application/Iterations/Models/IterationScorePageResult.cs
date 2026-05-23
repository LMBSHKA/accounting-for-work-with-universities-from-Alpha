using Entities.enums;

namespace Application.Iterations.Models;

public class IterationScorePageResult
{
	public Guid ProjectId { get; set; }
	public string ProjectTitle { get; set; } = string.Empty;
	public ProjectStatus ProjectStatus { get; set; }
	public string? ProjectEvaluationCriteria { get; set; }
	public Guid? SelectedTeamId { get; set; }
	public string? SelectedTeamName { get; set; }
	public Guid? SelectedIterationId { get; set; }
	public string? SelectedIterationName { get; set; }
	public string? SelectedIterationEvaluationCriteria { get; set; }
	public List<IterationResult> Iterations { get; set; } = [];
	public List<IterationScoreMemberResult> Members { get; set; } = [];
}
