using Application.Authentication.Models;
using Entities.Models;

namespace Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    TokenResult CreateToken(User user);
}
