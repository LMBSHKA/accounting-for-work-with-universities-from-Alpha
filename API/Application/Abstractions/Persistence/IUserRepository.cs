using Entities.Models;

namespace Application.Abstractions.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetActiveByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
