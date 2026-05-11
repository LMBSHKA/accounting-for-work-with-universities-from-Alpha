using Application.Abstractions.Persistence;
using Application.Abstractions.Students;
using Application.Students.Models;
using Entities.Models;

namespace Application.Students.Services;

public class StudentService(IUnitOfWork unitOfWork) : IStudentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<StudentResult?> CreateAsync(CreateStudentCommand command, CancellationToken cancellationToken = default)
    {
        var fullName = NormalizeRequired(command.FullName);
        var email = NormalizeRequired(command.Email).ToLower();
        var roleInTeam = NormalizeOptional(command.RoleInTeam);

        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var existingStudent = await _unitOfWork.StudentProfiles.GetByEmailAsync(email, cancellationToken);
        if (existingStudent is not null)
        {
            return null;
        }

        var student = new StudentProfile
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            RoleInTeam = roleInTeam,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.StudentProfiles.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(student);
    }

    private static StudentResult Map(StudentProfile student)
    {
        return new StudentResult
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            RoleInTeam = student.RoleInTeam,
            UpdatedAt = student.UpdatedAt
        };
    }

    private static string NormalizeRequired(string value)
    {
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
