namespace API.Contracts.Iterations;

public class IterationResponse
{
	public Guid Id { get; set; }
	public Guid ProjectId { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateOnly StartOn { get; set; }
	public DateOnly EndOn { get; set; }
	public string? EvaluationCriteria { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
