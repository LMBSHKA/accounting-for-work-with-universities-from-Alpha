using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Authentication.Models;

namespace Application.Authentication.Services;

public class AuthService(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<AuthResult?> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetActiveByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            return null;
        }

        if (!string.Equals(user.Password, command.Password, StringComparison.Ordinal))
        {
            return null;
        }

        var tokenResult = _jwtTokenGenerator.CreateToken(user);

        return new AuthResult
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            SystemRole = user.SystemRole,
            Token = tokenResult
        };
    }
}
