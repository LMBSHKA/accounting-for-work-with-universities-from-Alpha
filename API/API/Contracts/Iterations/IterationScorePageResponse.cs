using Entities.enums;

namespace API.Contracts.Iterations;

public class IterationScorePageResponse
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
	public List<IterationResponse> Iterations { get; set; } = [];
	public List<IterationScoreMemberResponse> Members { get; set; } = [];
}
