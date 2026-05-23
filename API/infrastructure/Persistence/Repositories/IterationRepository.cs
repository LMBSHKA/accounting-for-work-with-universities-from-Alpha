using Application.Abstractions.Persistence;
using Application.Iterations.Models;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class IterationRepository(AppDbContext dbContext) : Repository<Iteration>(dbContext), IIterationRepository
{
	public async Task<IReadOnlyCollection<IterationResult>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		return await BuildIterationProjection(DbContext.Iterations.AsNoTracking().Where(iteration => iteration.ProjectId == projectId))
			.OrderBy(iteration => iteration.StartOn)
			.ThenBy(iteration => iteration.Name)
			.ToListAsync(cancellationToken);
	}

	public async Task<IterationResult?> GetIterationAsync(Guid iterationId, CancellationToken cancellationToken = default)
	{
		return await BuildIterationProjection(DbContext.Iterations.AsNoTracking())
			.FirstOrDefaultAsync(iteration => iteration.Id == iterationId, cancellationToken);
	}

	public async Task<IterationScorePageResult?> GetScorePageAsync(GetIterationScorePageQuery request, CancellationToken cancellationToken = default)
	{
		var project = await DbContext.Projects
			.AsNoTracking()
			.Where(project => project.Id == request.ProjectId)
			.Select(project => new
			{
				project.Id,
				project.Title,
				project.Status,
				EvaluationCriteria = project.Tasks
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (project is null)
		{
			return null;
		}

		var iterations = await BuildIterationProjection(DbContext.Iterations.AsNoTracking().Where(iteration => iteration.ProjectId == request.ProjectId))
			.OrderBy(iteration => iteration.StartOn)
			.ThenBy(iteration => iteration.Name)
			.ToListAsync(cancellationToken);

		var selectedIteration = request.IterationId.HasValue
			? iterations.FirstOrDefault(iteration => iteration.Id == request.IterationId.Value)
			: iterations.LastOrDefault();

		var selectedTeam = await DbContext.Teams
			.AsNoTracking()
			.Where(team => team.ProjectId == request.ProjectId)
			.Where(team => !request.TeamId.HasValue || team.Id == request.TeamId.Value)
			.OrderBy(team => team.Name)
			.Select(team => new
			{
				team.Id,
				team.Name
			})
			.FirstOrDefaultAsync(cancellationToken);

		var members = new List<IterationScoreMemberResult>();
		if (selectedTeam is not null && selectedIteration is not null)
		{
			members = await DbContext.TeamMembers
				.AsNoTracking()
				.Where(member => member.TeamId == selectedTeam.Id)
				.OrderBy(member => member.StudentsProfile == null ? string.Empty : member.StudentsProfile.FullName)
				.Select(member => new IterationScoreMemberResult
				{
					MemberId = member.Id,
					StudentProfileId = member.StudentsProfileId,
					FullName = member.StudentsProfile == null ? string.Empty : member.StudentsProfile.FullName,
					RoleInTeam = member.StudentsProfile == null ? null : member.StudentsProfile.RoleInTeam,
					Score = DbContext.IterationMemberScores
						.Where(score => score.IterationId == selectedIteration.Id && score.MemberId == member.Id)
						.Select(score => (int?)score.Score)
						.FirstOrDefault(),
					Comment = DbContext.IterationMemberScores
						.Where(score => score.IterationId == selectedIteration.Id && score.MemberId == member.Id)
						.Select(score => score.Comment)
						.FirstOrDefault()
				})
				.ToListAsync(cancellationToken);
		}

		return new IterationScorePageResult
		{
			ProjectId = project.Id,
			ProjectTitle = project.Title,
			ProjectStatus = project.Status,
			ProjectEvaluationCriteria = project.EvaluationCriteria,
			SelectedTeamId = selectedTeam?.Id,
			SelectedTeamName = selectedTeam?.Name,
			SelectedIterationId = selectedIteration?.Id,
			SelectedIterationName = selectedIteration?.Name,
			SelectedIterationEvaluationCriteria = selectedIteration?.EvaluationCriteria,
			Iterations = iterations,
			Members = members
		};
	}

	private static IQueryable<IterationResult> BuildIterationProjection(IQueryable<Iteration> query)
	{
		return query.Select(iteration => new IterationResult
		{
			Id = iteration.Id,
			ProjectId = iteration.ProjectId,
			Name = iteration.Name,
			StartOn = iteration.StartOn,
			EndOn = iteration.EndOn,
			EvaluationCriteria = iteration.EvaluationCriteria,
			CreatedAt = iteration.CreatedAt,
			UpdatedAt = iteration.UpdatedAt
		});
	}
}
