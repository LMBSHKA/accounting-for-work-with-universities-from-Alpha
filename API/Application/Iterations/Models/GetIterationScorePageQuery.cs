namespace Application.Iterations.Models;

public class GetIterationScorePageQuery
{
	public Guid ProjectId { get; set; }
	public Guid? TeamId { get; set; }
	public Guid? IterationId { get; set; }
}
