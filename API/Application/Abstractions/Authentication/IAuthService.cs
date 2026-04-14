using Application.Authentication.Models;

namespace Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<AuthResult?> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);
}
