using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Iterations;

public class SaveIterationMemberScoresRequest
{
	[Required]
	public Guid TeamId { get; set; }
	public List<SaveIterationMemberScoreItemRequest> Scores { get; set; } = [];
}
