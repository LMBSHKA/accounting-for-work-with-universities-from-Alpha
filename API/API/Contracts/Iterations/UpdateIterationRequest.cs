using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Iterations;

public class UpdateIterationRequest
{
	[MaxLength(200)]
	public string? Name { get; set; }
	public DateOnly? StartOn { get; set; }
	public DateOnly? EndOn { get; set; }

	[MaxLength(4000)]
	public string? EvaluationCriteria { get; set; }
}
