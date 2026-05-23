namespace Application.Iterations.Models;

public class SaveIterationMemberScoresCommand
{
	public Guid IterationId { get; set; }
	public Guid TeamId { get; set; }
	public Guid ScoredByUserId { get; set; }
	public List<SaveIterationMemberScoreItemCommand> Scores { get; set; } = [];
}
