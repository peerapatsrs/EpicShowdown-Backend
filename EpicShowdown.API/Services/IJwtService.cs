using System.Security.Claims;

namespace EpicShowdown.API.Services;

public interface IJwtService
{
    string GenerateAccessToken(Claim[] claims);
    string GenerateRefreshToken();
}