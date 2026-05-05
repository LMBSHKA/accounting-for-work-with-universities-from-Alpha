using Application.Discussions.Models;
using Application.Projects.Models;
using Entities.Models;

namespace Application.Abstractions.Persistence
{
	public interface IProjectRepository : IRepository<Project>
	{
		Task<GetProjectsResult> GetProjectsAsync(GetProjectsQuery query, CancellationToken cancellationToken = default);
		Task<GetDiscussionIdeasResult> GetDiscussionIdeasAsync(GetDiscussionIdeasQuery query, CancellationToken cancellationToken = default);
	}
}
