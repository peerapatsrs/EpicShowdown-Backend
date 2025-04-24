using Microsoft.AspNetCore.Mvc;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Repositories;
using EpicShowdown.API.Models.Redis;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EpicShowdown.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(
        IUserRepository userRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = CreateUserFromRequest(request);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var (accessToken, refreshToken) = await GenerateTokensAsync(user);

        return Ok(new
        {
            accessToken,
            refreshToken = refreshToken.Token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !user.VerifyPassword(request.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var (accessToken, refreshToken) = await GenerateTokensAsync(user);

        return Ok(new
        {
            accessToken,
            refreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
        if (refreshToken == null)
        {
            return BadRequest(new { message = "Invalid refresh token" });
        }

        if (refreshToken.IsRevoked)
        {
            return BadRequest(new { message = "Refresh token has been revoked" });
        }

        if (refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Refresh token has expired" });
        }

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            return BadRequest(new { message = "User not found" });
        }

        var (accessToken, newRefreshToken) = await GenerateTokensAsync(user);
        await _refreshTokenService.RotateRefreshTokenAsync(request.RefreshToken, GetIpAddress());

        return Ok(new
        {
            accessToken,
            refreshToken = newRefreshToken.Token
        });
    }

    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
        if (refreshToken == null)
        {
            return BadRequest(new { message = "Invalid refresh token" });
        }

        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, GetIpAddress(), request.Reason);
        return Ok(new { message = "Token revoked successfully" });
    }

    private string GetIpAddress()
    {
        var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
            HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ??
            "unknown";
        return ipAddress;
    }

    private User CreateUserFromRequest(RegisterRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address
        };

        user.SetPassword(request.Password);
        return user;
    }

    private async Task<(string accessToken, RefreshToken refreshToken)> GenerateTokensAsync(User user)
    {
        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Username", user.Username),
            new Claim("UserRole", user.Role)
        };

        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, GetIpAddress());

        return (accessToken, refreshToken);
    }
}