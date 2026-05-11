using Application.Students.Models;
using Entities.Models;

namespace Application.Abstractions.Persistence;

public interface IStudentProfileRepository : IRepository<StudentProfile>
{
	Task<StudentProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<GetStudentsResult> GetStudentsAsync(GetStudentsQuery query, CancellationToken cancellationToken = default);
}
