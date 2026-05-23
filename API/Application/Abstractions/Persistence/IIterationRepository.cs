using Application.Iterations.Models;
using Entities.Models;

namespace Application.Abstractions.Persistence;

public interface IIterationRepository : IRepository<Iteration>
{
	Task<IReadOnlyCollection<IterationResult>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
	Task<IterationResult?> GetIterationAsync(Guid iterationId, CancellationToken cancellationToken = default);
	Task<IterationScorePageResult?> GetScorePageAsync(GetIterationScorePageQuery query, CancellationToken cancellationToken = default);
}
