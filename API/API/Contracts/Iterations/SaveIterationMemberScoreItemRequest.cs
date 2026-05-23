using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Iterations;

public class SaveIterationMemberScoreItemRequest
{
	[Required]
	public Guid MemberId { get; set; }

	[Range(0, 100)]
	public int Score { get; set; }

	[MaxLength(4000)]
	public string? Comment { get; set; }
}
