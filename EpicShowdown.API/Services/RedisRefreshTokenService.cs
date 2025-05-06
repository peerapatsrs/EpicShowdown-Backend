using System.Text.Json;
using EpicShowdown.API.Models.Redis;
using EpicShowdown.API.Repositories;
using StackExchange.Redis;

namespace EpicShowdown.API.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(int userId, string ipAddress);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token, string ipAddress, string? reason = null);
    Task<RefreshToken> RotateRefreshTokenAsync(string token, string ipAddress);
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private const string KeyPrefix = "refresh_token:";

    public RefreshTokenService(
        IConnectionMultiplexer redis,
        IUserRepository userRepository,
        IConfiguration configuration)
    {
        _redis = redis;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId, string ipAddress)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found");

        var expirationMinutes = Convert.ToInt32(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            ExpiryDate = DateTime.UtcNow.AddMinutes(expirationMinutes),
            CreatedDate = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            UserId = user.Id
        };

        var db = _redis.GetDatabase();
        var key = $"{KeyPrefix}{refreshToken.Token}";
        var value = JsonSerializer.Serialize(refreshToken);

        await db.StringSetAsync(key, value, refreshToken.ExpiryDate - DateTime.UtcNow);

        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        var db = _redis.GetDatabase();
        var key = $"{KeyPrefix}{token}";
        var value = await db.StringGetAsync(key);

        if (!value.HasValue)
            return null;

        var refreshToken = JsonSerializer.Deserialize<RefreshToken>(value!);
        if (refreshToken == null || refreshToken.IsRevoked)
            return null;

        return refreshToken;
    }

    public async Task RevokeRefreshTokenAsync(string token, string ipAddress, string? reason = null)
    {
        var refreshToken = await GetRefreshTokenAsync(token)
            ?? throw new InvalidOperationException("Refresh token not found");

        refreshToken.IsRevoked = true;
        refreshToken.RevokedDate = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;

        var db = _redis.GetDatabase();
        var key = $"{KeyPrefix}{token}";
        var value = JsonSerializer.Serialize(refreshToken);

        await db.StringSetAsync(key, value, refreshToken.ExpiryDate - DateTime.UtcNow);
    }

    public async Task<RefreshToken> RotateRefreshTokenAsync(string token, string ipAddress)
    {
        var oldRefreshToken = await GetRefreshTokenAsync(token)
            ?? throw new InvalidOperationException("Refresh token not found");

        // Revoke the old refresh token
        await RevokeRefreshTokenAsync(token, ipAddress, "Replaced by new token");

        // Generate a new refresh token
        var newRefreshToken = await GenerateRefreshTokenAsync(oldRefreshToken.UserId, ipAddress);

        // Link the new refresh token to the old one
        oldRefreshToken.ReplacedByToken = newRefreshToken.Token;

        var db = _redis.GetDatabase();
        var oldKey = $"{KeyPrefix}{token}";
        var oldValue = JsonSerializer.Serialize(oldRefreshToken);

        await db.StringSetAsync(oldKey, oldValue, oldRefreshToken.ExpiryDate - DateTime.UtcNow);

        return newRefreshToken;
    }
}