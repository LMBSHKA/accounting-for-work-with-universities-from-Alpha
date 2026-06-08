using Application.Students.Models;
using Entities.Models;

namespace Application.Abstractions.Persistence;

public interface ITeamRepository : IRepository<Team>
{
	Task<GetTeamsResult> GetTeamsAsync(GetTeamsQuery query, CancellationToken cancellationToken = default);
	Task<GetCuratorsResult> GetCuratorsAsync(GetCuratorsQuery query, CancellationToken cancellationToken = default);
	Task<TeamDetailsResult?> GetTeamDetailsAsync(Guid teamId, CancellationToken cancellationToken = default);
}
