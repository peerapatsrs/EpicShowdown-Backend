using EpicShowdown.API.Models.Redis;

namespace EpicShowdown.API.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(int userId, string ipAddress);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token, string ipAddress, string? reason = null);
    Task<RefreshToken> RotateRefreshTokenAsync(string token, string ipAddress);
}