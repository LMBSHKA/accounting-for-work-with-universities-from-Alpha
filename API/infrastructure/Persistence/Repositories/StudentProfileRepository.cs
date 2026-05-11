using Application.Abstractions.Persistence;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class StudentProfileRepository(AppDbContext dbContext) : Repository<StudentProfile>(dbContext), IStudentProfileRepository
{
    public Task<StudentProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();

        return DbSet.FirstOrDefaultAsync(student =>
            student.Email.ToLower() == normalizedEmail,
            cancellationToken);
    }
}
