using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Auth
{
	public class RegisterRequest
	{
		[Required, EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required, MinLength(4)]
		public string Password { get; set; } = string.Empty;

		[Required, MaxLength(256)]
		public string FullName { get; set; } = string.Empty;

		public string SystemRole { get; set; } = "Student";
	}
}
