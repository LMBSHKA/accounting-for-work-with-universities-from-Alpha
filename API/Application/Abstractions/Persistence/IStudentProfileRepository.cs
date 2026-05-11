using Entities.Models;

namespace Application.Abstractions.Persistence;

public interface IStudentProfileRepository : IRepository<StudentProfile>
{
    Task<StudentProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
