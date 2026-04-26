using Application.Abstractions.Persistence;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
	public async Task<User?> GetActiveByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var normalizedEmail = email.Trim().ToLowerInvariant();

		return await Query()
			.AsNoTracking()
			.FirstOrDefaultAsync(
				x => x.IsActive && x.Email.ToLower() == normalizedEmail,
				cancellationToken);
	}

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var normalizedEmail = email.Trim().ToLowerInvariant();

		return await Query()
			.AsNoTracking()
			.FirstOrDefaultAsync(
				x => x.Email.ToLower() == normalizedEmail,
				cancellationToken);
	}
}