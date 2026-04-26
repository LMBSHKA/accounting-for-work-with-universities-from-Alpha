using Application.Abstractions.Authentication.Models;
using Application.Authentication.Models;

namespace Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<AuthResult?> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);
	Task<AuthResult?> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default);
}
