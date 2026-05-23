using Application.Projects.Models;
using Entities.Models;

namespace Application.Abstractions.Projects;

public interface IProjectService
{
    Task<Project> CreateAsync(CreateProject command, CancellationToken cancellationToken = default);
	Task<GetProjectsResult> GetProjectsAsync(GetProjectsQuery query, CancellationToken cancellationToken = default);
	Task<bool> CompleteProjectAsync(Guid projectId, Guid changedByUserId, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<ProjectStatusHistoryResult>?> GetStatusHistoryAsync(Guid projectId, CancellationToken cancellationToken = default);
}
