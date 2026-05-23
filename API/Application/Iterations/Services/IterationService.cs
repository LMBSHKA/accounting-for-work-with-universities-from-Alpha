using Application.Abstractions.Iterations;
using Application.Abstractions.Persistence;
using Application.Iterations.Models;
using Entities.Models;

namespace Application.Iterations.Services;

public class IterationService(IUnitOfWork unitOfWork) : IIterationService
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task<IterationResult?> CreateAsync(CreateIterationCommand command, CancellationToken cancellationToken = default)
	{
		var name = NormalizeRequired(command.Name);
		if (command.ProjectId == Guid.Empty || string.IsNullOrWhiteSpace(name) || command.EndOn < command.StartOn)
		{
			return null;
		}

		var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken);
		if (project is null)
		{
			return null;
		}

		var exists = _unitOfWork.Iterations.Query()
			.Any(iteration => iteration.ProjectId == command.ProjectId && iteration.Name == name);
		if (exists)
		{
			return null;
		}

		var iteration = new Iteration
		{
			Id = Guid.NewGuid(),
			ProjectId = command.ProjectId,
			Name = name,
			StartOn = command.StartOn,
			EndOn = command.EndOn,
			EvaluationCriteria = NormalizeOptional(command.EvaluationCriteria),
			CreatedAt = DateTime.UtcNow
		};

		await _unitOfWork.Iterations.AddAsync(iteration, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return await _unitOfWork.Iterations.GetIterationAsync(iteration.Id, cancellationToken);
	}

	public async Task<IterationResult?> UpdateAsync(UpdateIterationCommand command, CancellationToken cancellationToken = default)
	{
		var iteration = await _unitOfWork.Iterations.GetByIdAsync(command.IterationId, cancellationToken);
		if (iteration is null)
		{
			return null;
		}

		var startOn = command.StartOn ?? iteration.StartOn;
		var endOn = command.EndOn ?? iteration.EndOn;
		if (endOn < startOn)
		{
			return null;
		}

		if (command.Name is not null)
		{
			var name = NormalizeRequired(command.Name);
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			var sameNameExists = _unitOfWork.Iterations.Query()
				.Any(item => item.ProjectId == iteration.ProjectId && item.Id != iteration.Id && item.Name == name);
			if (sameNameExists)
			{
				return null;
			}

			iteration.Name = name;
		}

		iteration.StartOn = startOn;
		iteration.EndOn = endOn;
		if (command.EvaluationCriteria is not null)
		{
			iteration.EvaluationCriteria = NormalizeOptional(command.EvaluationCriteria);
		}

		iteration.UpdatedAt = DateTime.UtcNow;
		_unitOfWork.Iterations.Update(iteration);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return await _unitOfWork.Iterations.GetIterationAsync(iteration.Id, cancellationToken);
	}

	public Task<IReadOnlyCollection<IterationResult>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.Iterations.GetByProjectAsync(projectId, cancellationToken);
	}

	public Task<IterationScorePageResult?> GetScorePageAsync(GetIterationScorePageQuery query, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.Iterations.GetScorePageAsync(query, cancellationToken);
	}

	public async Task<bool> SaveMemberScoresAsync(SaveIterationMemberScoresCommand command, CancellationToken cancellationToken = default)
	{
		if (command.IterationId == Guid.Empty || command.TeamId == Guid.Empty || command.ScoredByUserId == Guid.Empty)
		{
			return false;
		}

		var iteration = await _unitOfWork.Iterations.GetByIdAsync(command.IterationId, cancellationToken);
		var team = await _unitOfWork.Teams.GetByIdAsync(command.TeamId, cancellationToken);
		if (iteration is null || team is null || team.ProjectId != iteration.ProjectId)
		{
			return false;
		}

		var memberIds = command.Scores.Select(score => score.MemberId).Distinct().ToList();
		var teamMemberIds = _unitOfWork.TeamMembers.Query()
			.Where(member => member.TeamId == command.TeamId && memberIds.Contains(member.Id))
			.Select(member => member.Id)
			.ToHashSet();

		if (teamMemberIds.Count != memberIds.Count)
		{
			return false;
		}

		var now = DateTime.UtcNow;
		foreach (var item in command.Scores)
		{
			if (item.Score is < 0 or > 100)
			{
				return false;
			}

			var existingScore = _unitOfWork.IterationMemberScores.Query()
				.FirstOrDefault(score => score.IterationId == command.IterationId && score.MemberId == item.MemberId);

			if (existingScore is null)
			{
				var score = new IterationMemberScore
				{
					Id = Guid.NewGuid(),
					IterationId = command.IterationId,
					MemberId = item.MemberId,
					Score = item.Score,
					Comment = NormalizeOptional(item.Comment),
					ScoredByUserId = command.ScoredByUserId,
					CreatedAt = now
				};

				await _unitOfWork.IterationMemberScores.AddAsync(score, cancellationToken);
			}
			else
			{
				existingScore.Score = item.Score;
				existingScore.Comment = NormalizeOptional(item.Comment);
				existingScore.ScoredByUserId = command.ScoredByUserId;
				_unitOfWork.IterationMemberScores.Update(existingScore);
			}
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return true;
	}

	private static string NormalizeRequired(string value)
	{
		return value.Trim();
	}

	private static string? NormalizeOptional(string? value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
	}
}
