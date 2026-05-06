namespace Entities.Models;

public class User
{
	public Guid Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public string SystemRole { get; set; } = string.Empty;
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }

	public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
	public ICollection<ProjectStatusHistory> ChangedProjectStatuses { get; set; } = new List<ProjectStatusHistory>();
	public ICollection<ProjectReaction> CreatedProjectReactions { get; set; } = new List<ProjectReaction>();
	public ICollection<ProjectComment> ProjectComments { get; set; } = new List<ProjectComment>();
	public ICollection<ProjectCommentReaction> ProjectCommentReactions { get; set; } = new List<ProjectCommentReaction>();
	public ICollection<Team> CreatedTeams { get; set; } = new List<Team>();
	public ICollection<Meeting> CreatedMeetings { get; set; } = new List<Meeting>();
	public ICollection<TeamResult> UploadedTeamResults { get; set; } = new List<TeamResult>();
	public ICollection<IterationTeamScore> ScoredIterationTeams { get; set; } = new List<IterationTeamScore>();
	public ICollection<IterationMemberScore> ScoredIterationMembers { get; set; } = new List<IterationMemberScore>();
	public ICollection<FinalTeamScore> ScoredFinalTeams { get; set; } = new List<FinalTeamScore>();
	public ICollection<FinalMemberScore> ScoredFinalMembers { get; set; } = new List<FinalMemberScore>();
}
