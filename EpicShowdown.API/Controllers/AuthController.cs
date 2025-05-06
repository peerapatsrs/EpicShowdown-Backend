using Microsoft.AspNetCore.Mvc;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Repositories;
using EpicShowdown.API.Models.Redis;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using EpicShowdown.API.Models.DTOs.Responses;

namespace EpicShowdown.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IPassKeyService _passKeyService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
        IUserRepository userRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IPassKeyService passKeyService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _passKeyService = passKeyService;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = CreateUserFromRequest(request);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var (accessToken, refreshToken) = await GenerateTokensAsync(user);

        return CreatedAtAction(nameof(Login), new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !user.VerifyPassword(request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var (accessToken, refreshToken) = await GenerateTokensAsync(user);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
        if (refreshToken == null)
        {
            return NotFound(new { message = "Invalid refresh token" });
        }

        if (refreshToken.IsRevoked)
        {
            return NotFound(new { message = "Refresh token has been revoked" });
        }

        if (refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return NotFound(new { message = "Refresh token has expired" });
        }

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var (accessToken, newRefreshToken) = await GenerateTokensAsync(user);
        await _refreshTokenService.RotateRefreshTokenAsync(request.RefreshToken, GetIpAddress());

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token
        });
    }

    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
        if (refreshToken == null)
        {
            return NotFound(new { message = "Invalid refresh token" });
        }

        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, GetIpAddress(), request.Reason);
        return Ok(new { message = "Token revoked successfully" });
    }

    [HttpPost("passkey/register/options")]
    [Authorize]
    public async Task<IActionResult> GetPassKeyRegistrationOptions()
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        if (user == null)
            return NotFound(new { message = "User not found" });

        var options = await _passKeyService.GenerateRegistrationOptionsAsync(user.UserCode, user.Email);
        return Ok(new { options });
    }

    [HttpPost("passkey/register/verify")]
    [Authorize]
    public async Task<IActionResult> VerifyPassKeyRegistration([FromBody] PassKeyRegistrationRequest request)
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        var result = await _passKeyService.VerifyRegistrationAsync(user?.UserCode ?? Guid.Empty, request);
        if (!result) return NotFound(new { message = "PassKey registration failed" });
        return Ok(new { message = "PassKey registered successfully" });
    }

    // อัพเดท endpoint ให้เรียกเมธอด async ใหม่
    [HttpPost("passkey/authenticate/options")]
    public async Task<IActionResult> GetPassKeyAuthenticationOptions()
    {
        var options = await _passKeyService.GenerateAuthenticationOptionsAsync();
        return Ok(new { options });
    }

    [HttpPost("passkey/authenticate")]
    public async Task<IActionResult> AuthenticateWithPassKey([FromBody] PassKeyAuthenticationRequest request)
    {
        var authResult = await _passKeyService.VerifyAuthenticationAsync(request);
        if (!authResult.Success || authResult.User?.Email == null)
            return Unauthorized(new { message = "PassKey authentication failed" });

        var user = await _userRepository.GetByEmailAsync(authResult.User.Email);
        if (user == null)
            return Unauthorized(new { message = "User not found" });

        var (accessToken, refreshToken) = await GenerateTokensAsync(user);
        return Ok(new LoginResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token });
    }

    [HttpDelete("passkey/{passKeyId}")]
    [Authorize]
    public async Task<IActionResult> RevokePassKey(string passKeyId)
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        if (user == null)
            return Unauthorized(new { message = "User not found" });

        var result = await _passKeyService.RevokePassKeyAsync(user.UserCode, passKeyId);
        if (!result) return BadRequest(new { message = "Failed to revoke PassKey" });
        return Ok(new { message = "PassKey revoked successfully" });
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
            Email = request.Email
        };

        user.SetPassword(request.Password);
        return user;
    }

    private async Task<(string accessToken, RefreshToken refreshToken)> GenerateTokensAsync(User user)
    {
        var claims = new[]
        {
            new Claim("UserCode", user.UserCode.ToString()),
            new Claim("Email", user.Email),
            new Claim("UserRole", user.Role)
        };

        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, GetIpAddress());

        return (accessToken, refreshToken);
    }
}