using Entities.Models;

namespace Application.Abstractions.Persistence;

public interface IUnitOfWork
{
	IUserRepository Users { get; }
	IProjectRepository Projects { get; }
	IRepository<ProjectStatusHistory> ProjectStatusHistory { get; }
	IRepository<ProjectReaction> ProjectReactions { get; }
	IRepository<ProjectComment> ProjectComments { get; }
	IRepository<ProjectCommentReaction> ProjectCommentReactions { get; }
	IRepository<Iteration> Iterations { get; }
	ITeamRepository Teams { get; }
	IRepository<TeamResult> TeamResults { get; }
	IRepository<Meeting> Meetings { get; }
	IRepository<MeetingTask> MeetingTasks { get; }
	IRepository<TeamMember> TeamMembers { get; }
	IStudentProfileRepository StudentProfiles { get; }
	IRepository<IterationTeamScore> IterationTeamScores { get; }
	IRepository<FinalTeamScore> FinalTeamScores { get; }
	IRepository<IterationMemberScore> IterationMemberScores { get; }
	IRepository<FinalMemberScore> FinalMemberScores { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
