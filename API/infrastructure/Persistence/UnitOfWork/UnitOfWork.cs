using Application.Abstractions.Persistence;
using Entities.Models;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
	private readonly AppDbContext _dbContext = dbContext;

	public IUserRepository Users { get; } = new UserRepository(dbContext);
	public IProjectRepository Projects { get; } = new ProjectRepository(dbContext);
	public IRepository<ProjectStatusHistory> ProjectStatusHistory { get; } = new Repository<ProjectStatusHistory>(dbContext);
	public IRepository<ProjectReaction> ProjectReactions { get; } = new Repository<ProjectReaction>(dbContext);
	public IRepository<ProjectComment> ProjectComments { get; } = new Repository<ProjectComment>(dbContext);
	public IRepository<ProjectCommentReaction> ProjectCommentReactions { get; } = new Repository<ProjectCommentReaction>(dbContext);
	public IIterationRepository Iterations { get; } = new IterationRepository(dbContext);
	public ITeamRepository Teams { get; } = new TeamRepository(dbContext);
	public IRepository<TeamResult> TeamResults { get; } = new Repository<TeamResult>(dbContext);
	public IMeetingRepository Meetings { get; } = new MeetingRepository(dbContext);
	public IRepository<MeetingTask> MeetingTasks { get; } = new Repository<MeetingTask>(dbContext);
	public IRepository<TeamMember> TeamMembers { get; } = new Repository<TeamMember>(dbContext);
	public IStudentProfileRepository StudentProfiles { get; } = new StudentProfileRepository(dbContext);
	public IRepository<IterationTeamScore> IterationTeamScores { get; } = new Repository<IterationTeamScore>(dbContext);
	public IRepository<FinalTeamScore> FinalTeamScores { get; } = new Repository<FinalTeamScore>(dbContext);
	public IRepository<IterationMemberScore> IterationMemberScores { get; } = new Repository<IterationMemberScore>(dbContext);
	public IRepository<FinalMemberScore> FinalMemberScores { get; } = new Repository<FinalMemberScore>(dbContext);


	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return _dbContext.SaveChangesAsync(cancellationToken);
	}
}
