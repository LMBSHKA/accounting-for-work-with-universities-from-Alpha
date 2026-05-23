namespace Application.Iterations.Models;

public class SaveIterationMemberScoreItemCommand
{
	public Guid MemberId { get; set; }
	public int Score { get; set; }
	public string? Comment { get; set; }
}
