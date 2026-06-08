using Entities.enums;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<User> Users => Set<User>();
	public DbSet<Project> Projects => Set<Project>();
	public DbSet<ProjectStatusHistory> ProjectStatusHistory => Set<ProjectStatusHistory>();
	public DbSet<ProjectReaction> ProjectReactions => Set<ProjectReaction>();
	public DbSet<ProjectComment> ProjectComments => Set<ProjectComment>();
	public DbSet<ProjectCommentReaction> ProjectCommentReactions => Set<ProjectCommentReaction>();
	public DbSet<Iteration> Iterations => Set<Iteration>();
	public DbSet<Team> Teams => Set<Team>();
	public DbSet<TeamResult> TeamResults => Set<TeamResult>();
	public DbSet<Meeting> Meetings => Set<Meeting>();
	public DbSet<MeetingTask> MeetingTasks => Set<MeetingTask>();
	public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
	public DbSet<StudentProfile> StudentsProfiles => Set<StudentProfile>();
	public DbSet<IterationTeamScore> IterationTeamScores => Set<IterationTeamScore>();
	public DbSet<FinalTeamScore> FinalTeamScores => Set<FinalTeamScore>();
	public DbSet<IterationMemberScore> IterationMemberScores => Set<IterationMemberScore>();
	public DbSet<FinalMemberScore> FinalMemberScores => Set<FinalMemberScore>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		ConfigureUsers(modelBuilder);
		ConfigureProjects(modelBuilder);
		ConfigureProjectStatusHistory(modelBuilder);
		ConfigureProjectReactions(modelBuilder);
		ConfigureProjectComments(modelBuilder);
		ConfigureProjectCommentReactions(modelBuilder);
		ConfigureIterations(modelBuilder);
		ConfigureTeams(modelBuilder);
		ConfigureTeamResults(modelBuilder);
		ConfigureMeetings(modelBuilder);
		ConfigureMeetingTasks(modelBuilder);
		ConfigureStudentProfiles(modelBuilder);
		ConfigureTeamMembers(modelBuilder);
		ConfigureIterationTeamScores(modelBuilder);
		ConfigureFinalTeamScores(modelBuilder);
		ConfigureIterationMemberScores(modelBuilder);
		ConfigureFinalMemberScores(modelBuilder);
	}

	private static void ConfigureUsers(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(entity =>
		{
			entity.ToTable("Users");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
			entity.Property(e => e.Password).HasMaxLength(512).IsRequired();
			entity.Property(e => e.FullName).HasMaxLength(256).IsRequired();
			entity.Property(e => e.SystemRole).HasMaxLength(100).IsRequired();

			entity.HasIndex(e => e.Email).IsUnique();
		});
	}

	private static void ConfigureProjects(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Project>(entity =>
		{
			entity.ToTable("Projects");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Title).HasMaxLength(250).IsRequired();
			entity.Property(e => e.Status).HasMaxLength(100).IsRequired();

			entity.HasOne(e => e.CreatedByUser)
				.WithMany(u => u.CreatedProjects)
				.HasForeignKey(e => e.CreatedByUserId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}

	private static void ConfigureProjectStatusHistory(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProjectStatusHistory>(entity =>
		{
			entity.ToTable("ProjectStatusHistory");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Status).HasMaxLength(100).IsRequired();
			entity.Property(e => e.ChangeComment).HasMaxLength(1000);

			entity.HasOne(e => e.Project)
				.WithMany(p => p.StatusHistory)
				.HasForeignKey(e => e.ProjectId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.ChangedByUser)
				.WithMany(u => u.ChangedProjectStatuses)
				.HasForeignKey(e => e.ChangedByUserId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}

	private static void ConfigureProjectReactions(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProjectReaction>(entity =>
		{
			entity.ToTable("ProjectReactions");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.ReactionType)
				.HasConversion<int>()
				.IsRequired();
			entity.Property(e => e.Description).HasMaxLength(500);

			entity.HasOne(e => e.Project)
				.WithMany(p => p.Reactions)
				.HasForeignKey(e => e.ProjectId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.CreatedByUser)
				.WithMany(u => u.CreatedProjectReactions)
				.HasForeignKey(e => e.CreatedByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(e => new { e.ProjectId, e.CreatedByUserId }).IsUnique();
		});
	}

	private static void ConfigureProjectComments(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProjectComment>(entity =>
		{
			entity.ToTable("ProjectComments");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.CommentBody).HasMaxLength(4000).IsRequired();

			entity.HasOne(e => e.Project)
				.WithMany(p => p.Comments)
				.HasForeignKey(e => e.ProjectId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.User)
				.WithMany(u => u.ProjectComments)
				.HasForeignKey(e => e.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasOne(e => e.ParentComment)
				.WithMany(p => p.Replies)
				.HasForeignKey(e => e.ParentCommentId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}


	private static void ConfigureProjectCommentReactions(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProjectCommentReaction>(entity =>
		{
			entity.ToTable("ProjectCommentReactions");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.ReactionType)
				.HasConversion<int>()
				.IsRequired();

			entity.HasOne(e => e.ProjectComment)
				.WithMany(c => c.Reactions)
				.HasForeignKey(e => e.ProjectCommentId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.User)
				.WithMany(u => u.ProjectCommentReactions)
				.HasForeignKey(e => e.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(e => new { e.ProjectCommentId, e.UserId }).IsUnique();
		});
	}

	private static void ConfigureIterations(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Iteration>(entity =>
		{
			entity.ToTable("Iterations");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
			entity.Property(e => e.EvaluationCriteria).HasMaxLength(4000);

			entity.HasOne(e => e.Project)
				.WithMany(p => p.Iterations)
				.HasForeignKey(e => e.ProjectId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasIndex(e => new { e.ProjectId, e.Name }).IsUnique();
		});
	}

	private static void ConfigureTeams(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Team>(entity =>
		{
			entity.ToTable("Teams");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
			entity.Property(e => e.Skills).HasMaxLength(1000);
			entity.Property(e => e.FileUrl).HasMaxLength(1024);

			entity.HasOne(e => e.Project)
				.WithMany(p => p.Teams)
				.HasForeignKey(e => e.ProjectId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.CreatedByUser)
				.WithMany(u => u.CreatedTeams)
				.HasForeignKey(e => e.CreatedByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(e => new { e.ProjectId, e.Name }).IsUnique();
		});
	}

	private static void ConfigureTeamResults(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TeamResult>(entity =>
		{
			entity.ToTable("TeamResults");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.FilePath).HasMaxLength(1024).IsRequired();

			entity.HasOne(e => e.Project)
				.WithMany(p => p.TeamResults)
				.HasForeignKey(e => e.ProjectId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.Team)
				.WithMany(t => t.Results)
				.HasForeignKey(e => e.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.UploadedByUser)
				.WithMany(u => u.UploadedTeamResults)
				.HasForeignKey(e => e.UploadedByUserId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}

	private static void ConfigureMeetings(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Meeting>(entity =>
		{
			entity.ToTable("Meetings");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Title).HasMaxLength(250).IsRequired();
			entity.Property(e => e.Description).HasMaxLength(4000);
			entity.Property(e => e.Location).HasMaxLength(500);
			entity.Property(e => e.ConnectionLink).HasMaxLength(1024);

			entity.HasOne(e => e.Team)
				.WithMany(t => t.Meetings)
				.HasForeignKey(e => e.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.CreatedByUser)
				.WithMany(u => u.CreatedMeetings)
				.HasForeignKey(e => e.CreatedByUserId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}

	private static void ConfigureMeetingTasks(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MeetingTask>(entity =>
		{
			entity.ToTable("MeetingTasks");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Title).HasMaxLength(300).IsRequired();
			entity.Property(e => e.Status).HasMaxLength(100).IsRequired();
			entity.Property(e => e.Description).HasMaxLength(4000);

			entity.HasOne(e => e.Meeting)
				.WithMany(m => m.Tasks)
				.HasForeignKey(e => e.MeetingId)
				.OnDelete(DeleteBehavior.Cascade);
		});
	}

	private static void ConfigureStudentProfiles(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<StudentProfile>(entity =>
		{
			entity.ToTable("StudentsProfile");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.FullName).HasMaxLength(256).IsRequired();
			entity.Property(e => e.RoleInTeam).HasMaxLength(150);
			entity.Property(e => e.Email).HasMaxLength(256).IsRequired();

			entity.HasIndex(e => e.Email).IsUnique();
		});
	}

	private static void ConfigureTeamMembers(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TeamMember>(entity =>
		{
			entity.ToTable("TeamMembers");
			entity.HasKey(e => e.Id);

			entity.HasOne(e => e.Team)
				.WithMany(t => t.Members)
				.HasForeignKey(e => e.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.StudentsProfile)
				.WithMany(s => s.TeamMemberships)
				.HasForeignKey(e => e.StudentsProfileId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(e => new { e.TeamId, e.StudentsProfileId }).IsUnique();
		});
	}

	private static void ConfigureIterationTeamScores(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<IterationTeamScore>(entity =>
		{
			entity.ToTable("IterationTeamScores");
			entity.HasKey(e => e.Id);

			entity.HasOne(e => e.Team)
				.WithMany(t => t.IterationScores)
				.HasForeignKey(e => e.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.Iteration)
				.WithMany(i => i.TeamScores)
				.HasForeignKey(e => e.IterationId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.ScoredByUser)
				.WithMany(u => u.ScoredIterationTeams)
				.HasForeignKey(e => e.ScoredByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(e => new { e.IterationId, e.TeamId }).IsUnique();
		});
	}

	private static void ConfigureFinalTeamScores(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<FinalTeamScore>(entity =>
		{
			entity.ToTable("FinalTeamScores");
			entity.HasKey(e => e.Id);

			entity.HasOne(e => e.Team)
				.WithMany(t => t.FinalScores)
				.HasForeignKey(e => e.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.ScoredByUser)
				.WithMany(u => u.ScoredFinalTeams)
				.HasForeignKey(e => e.ScoredByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(e => e.TeamId).IsUnique();
		});
	}

	private static void ConfigureIterationMemberScores(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<IterationMemberScore>(entity =>
		{
			entity.ToTable("IterationMemberScores");
			entity.HasKey(e => e.Id);

			entity.HasOne(e => e.Iteration)
				.WithMany(i => i.MemberScores)
				.HasForeignKey(e => e.IterationId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.Member)
				.WithMany(m => m.IterationScores)
				.HasForeignKey(e => e.MemberId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.ScoredByUser)
				.WithMany(u => u.ScoredIterationMembers)
				.HasForeignKey(e => e.ScoredByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(e => new { e.IterationId, e.MemberId }).IsUnique();
		});
	}

	private static void ConfigureFinalMemberScores(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<FinalMemberScore>(entity =>
		{
			entity.ToTable("FinalMemberScores");
			entity.HasKey(e => e.Id);

			entity.HasOne(e => e.Iteration)
				.WithMany(i => i.FinalMemberScores)
				.HasForeignKey(e => e.IterationId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.StudentProfile)
				.WithMany(s => s.FinalScores)
				.HasForeignKey(e => e.StudentProfileId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasOne(e => e.ScoredByUser)
				.WithMany(u => u.ScoredFinalMembers)
				.HasForeignKey(e => e.ScoredByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(e => new { e.IterationId, e.StudentProfileId }).IsUnique();
		});
	}
}
