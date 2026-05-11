using Application.Students.Models;

namespace Application.Abstractions.Students;

public interface IStudentService
{
    Task<StudentResult?> CreateAsync(CreateStudentCommand command, CancellationToken cancellationToken = default);
}
