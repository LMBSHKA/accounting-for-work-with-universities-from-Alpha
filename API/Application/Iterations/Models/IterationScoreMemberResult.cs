namespace Application.Iterations.Models;

public class IterationScoreMemberResult
{
	public Guid MemberId { get; set; }
	public Guid StudentProfileId { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string? RoleInTeam { get; set; }
	public int? Score { get; set; }
	public string? Comment { get; set; }
}
