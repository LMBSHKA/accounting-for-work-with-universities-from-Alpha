namespace Application.Authentication.Models;

public class AuthResult
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string SystemRole { get; set; } = string.Empty;
    public TokenResult Token { get; set; } = new();
}
