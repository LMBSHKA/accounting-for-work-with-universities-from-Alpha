namespace API.Contracts.Auth;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string SystemRole { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
