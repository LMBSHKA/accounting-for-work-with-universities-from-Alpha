namespace Application.Iterations.Models;

public class CreateIterationCommand
{
	public Guid ProjectId { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateOnly StartOn { get; set; }
	public DateOnly EndOn { get; set; }
	public string? EvaluationCriteria { get; set; }
}
