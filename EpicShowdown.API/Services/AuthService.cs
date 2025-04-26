using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.Redis;
using EpicShowdown.API.Repositories;

namespace EpicShowdown.API.Services
{
    /// <summary>
    /// Service for handling authentication and user management
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="request">The registration request</param>
        /// <param name="ipAddress">The IP address of the user</param>
        /// <returns>Access token and refresh token</returns>
        Task<(string accessToken, string refreshToken)> RegisterAsync(RegisterRequest request, string ipAddress);

        /// <summary>
        /// Authenticates a user and generates tokens
        /// </summary>
        /// <param name="request">The login request</param>
        /// <param name="ipAddress">The IP address of the user</param>
        /// <returns>Access token and refresh token</returns>
        Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request, string ipAddress);

        /// <summary>
        /// Refreshes the access token using a refresh token
        /// </summary>
        /// <param name="request">The refresh token request</param>
        /// <param name="ipAddress">The IP address of the user</param>
        /// <returns>New access token and refresh token</returns>
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);

        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        /// <param name="request">The revoke token request</param>
        /// <param name="ipAddress">The IP address of the user</param>
        Task RevokeTokenAsync(RevokeTokenRequest request, string ipAddress);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<(string accessToken, string refreshToken)> RegisterAsync(RegisterRequest request, string ipAddress)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new ArgumentException("Email already exists");
            }

            var user = new User
            {
                Email = request.Email,
                UserCode = Guid.NewGuid()
            };
            user.SetPassword(request.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return await GenerateTokensAsync(user, ipAddress);
        }

        public async Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request, string ipAddress)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !user.VerifyPassword(request.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            return await GenerateTokensAsync(user, ipAddress);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
        {
            var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null)
            {
                throw new ArgumentException("Invalid refresh token");
            }

            if (refreshToken.IsRevoked)
            {
                throw new ArgumentException("Refresh token has been revoked");
            }

            if (refreshToken.ExpiryDate < DateTime.UtcNow)
            {
                throw new ArgumentException("Refresh token has expired");
            }

            var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var (accessToken, newRefreshToken) = await GenerateTokensAsync(user, ipAddress);
            await _refreshTokenService.RotateRefreshTokenAsync(request.RefreshToken, ipAddress);

            return (accessToken, newRefreshToken);
        }

        public async Task RevokeTokenAsync(RevokeTokenRequest request, string ipAddress)
        {
            var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null)
            {
                throw new ArgumentException("Invalid refresh token");
            }

            await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, ipAddress, request.Reason);
        }

        private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user, string ipAddress)
        {
            var claims = new[]
            {
                new Claim("UserCode", user.UserCode.ToString()),
                new Claim("Email", user.Email),
                new Claim("UserRole", user.Role)
            };

            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, ipAddress);

            return (accessToken, refreshToken.Token);
        }
    }
}