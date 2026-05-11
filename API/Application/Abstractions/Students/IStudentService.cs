using Application.Students.Models;

namespace Application.Abstractions.Students;

public interface IStudentService
{
	Task<StudentResult?> CreateAsync(CreateStudentCommand command, CancellationToken cancellationToken = default);
	Task<Application.Students.Models.TeamResult?> CreateTeamAsync(CreateTeamCommand command, CancellationToken cancellationToken = default);
	Task<GetStudentsResult> GetStudentsAsync(GetStudentsQuery query, CancellationToken cancellationToken = default);
	Task<GetTeamsResult> GetTeamsAsync(GetTeamsQuery query, CancellationToken cancellationToken = default);
}
