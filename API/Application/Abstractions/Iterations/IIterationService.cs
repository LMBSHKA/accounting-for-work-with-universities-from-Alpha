using Application.Iterations.Models;

namespace Application.Abstractions.Iterations;

public interface IIterationService
{
	Task<IterationResult?> CreateAsync(CreateIterationCommand command, CancellationToken cancellationToken = default);
	Task<IterationResult?> UpdateAsync(UpdateIterationCommand command, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<IterationResult>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
	Task<IterationScorePageResult?> GetScorePageAsync(GetIterationScorePageQuery query, CancellationToken cancellationToken = default);
	Task<bool> SaveMemberScoresAsync(SaveIterationMemberScoresCommand command, CancellationToken cancellationToken = default);
}
