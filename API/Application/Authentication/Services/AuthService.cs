using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.Models;
using Application.Abstractions.Persistence;
using Application.Authentication.Models;
using Entities.Models;

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
			return null;

		if (!string.Equals(user.Password, command.Password, StringComparison.Ordinal))
			return null;

		return CreateAuthResult(user);
	}

	public async Task<AuthResult?> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default)
	{
		var normalizedEmail = command.Email.Trim().ToLowerInvariant();

		var existingUser = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail, cancellationToken);
		if (existingUser is not null)
			return null;

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = normalizedEmail,
			Password = command.Password,
			FullName = command.FullName.Trim(),
			SystemRole = string.IsNullOrWhiteSpace(command.SystemRole)
				? "Student"
				: command.SystemRole.Trim(),
			IsActive = true,
			CreatedAt = DateTime.UtcNow
		};

		await _unitOfWork.Users.AddAsync(user, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return CreateAuthResult(user);
	}

	private AuthResult CreateAuthResult(User user)
	{
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