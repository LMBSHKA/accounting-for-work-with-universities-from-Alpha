namespace API.Contracts.Iterations;

public class GetIterationScorePageRequest
{
	public Guid ProjectId { get; set; }
	public Guid? TeamId { get; set; }
	public Guid? IterationId { get; set; }
}
