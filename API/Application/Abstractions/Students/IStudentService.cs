using Application.Students.Models;
using Entities.Models;

namespace Application.Abstractions.Students;

public interface IStudentService
{
	Task<StudentResult?> CreateAsync(CreateStudentCommand command, CancellationToken cancellationToken = default);
	Task<Application.Students.Models.TeamResult?> CreateTeamAsync(CreateTeamCommand command, CancellationToken cancellationToken = default);
}
