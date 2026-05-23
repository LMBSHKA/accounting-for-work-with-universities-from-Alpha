using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Iterations;

public class CreateIterationRequest
{
	[Required]
	public Guid ProjectId { get; set; }

	[Required]
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	[Required]
	public DateOnly StartOn { get; set; }

	[Required]
	public DateOnly EndOn { get; set; }

	[MaxLength(4000)]
	public string? EvaluationCriteria { get; set; }
}
