using API.Contracts.Auth;
using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.Models;
using Application.Authentication.Models;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

	[EndpointSummary("Вход в систему")]
	[AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var authResult = await _authService.LoginAsync(new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        }, cancellationToken);

        if (authResult is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        Response.Cookies.Append(_jwtOptions.CookieName, authResult.Token.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = authResult.Token.ExpiresAtUtc,
            IsEssential = true
        });

        return Ok(new AuthResponse
        {
            UserId = authResult.UserId,
            Email = authResult.Email,
            FullName = authResult.FullName,
            SystemRole = authResult.SystemRole,
            ExpiresAtUtc = authResult.Token.ExpiresAtUtc
        });
    }

	[EndpointSummary("Выход")]
	[Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(_jwtOptions.CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            IsEssential = true
        });

        return Ok(new { message = "Logged out." });
    }

	[EndpointSummary("Запрос для теста")]
	[Authorize]
    [HttpGet("me")]
    public ActionResult<CurrentUserResponse> Me()
    {
        var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new { message = "User identifier claim is missing." });
        }

        return Ok(new CurrentUserResponse
        {
            UserId = userId,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? string.Empty,
            FullName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            SystemRole = User.FindFirstValue("systemRole") ?? User.FindFirstValue(ClaimTypes.Role) ?? string.Empty
        });
    }

	[EndpointSummary("Тестовая регистрация пользователя")]
	[AllowAnonymous]
	[HttpPost("register")]
	public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var authResult = await _authService.RegisterAsync(new RegisterCommand
		{
			Email = request.Email,
			Password = request.Password,
			FullName = request.FullName,
			SystemRole = request.SystemRole
		}, cancellationToken);

		if (authResult is null)
		{
			return Conflict(new { message = "User with this email already exists." });
		}

		Response.Cookies.Append(_jwtOptions.CookieName, authResult.Token.AccessToken, new CookieOptions
		{
			HttpOnly = true,
			Secure = Request.IsHttps,
			SameSite = SameSiteMode.Lax,
			Expires = authResult.Token.ExpiresAtUtc,
			IsEssential = true
		});

		return Ok(new AuthResponse
		{
			UserId = authResult.UserId,
			Email = authResult.Email,
			FullName = authResult.FullName,
			SystemRole = authResult.SystemRole,
			ExpiresAtUtc = authResult.Token.ExpiresAtUtc
		});
	}
}
