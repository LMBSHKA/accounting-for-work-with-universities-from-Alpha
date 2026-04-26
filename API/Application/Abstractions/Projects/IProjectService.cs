using Application.Projects.Models;
using Entities.Models;

namespace Application.Abstractions.Projects;

public interface IProjectService
{
    Task<Project> CreateAsync(CreateProject command, CancellationToken cancellationToken = default);
}
