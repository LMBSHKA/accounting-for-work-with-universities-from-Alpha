namespace Application.Iterations.Models;

public class UpdateIterationCommand
{
	public Guid IterationId { get; set; }
	public string? Name { get; set; }
	public DateOnly? StartOn { get; set; }
	public DateOnly? EndOn { get; set; }
	public string? EvaluationCriteria { get; set; }
}
