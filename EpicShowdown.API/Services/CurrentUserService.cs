using System.Security.Claims;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Repositories;
using Microsoft.AspNetCore.Http;

namespace EpicShowdown.API.Services;

public interface ICurrentUserService
{
    Task<User?> GetCurrentUserAsync();
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private User? _currentUser;

    public CurrentUserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_currentUser != null) return _currentUser;

        var userCodeClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserCode")?.Value;
        if (string.IsNullOrEmpty(userCodeClaim) || !Guid.TryParse(userCodeClaim, out Guid userCode))
        {
            return null;
        }

        _currentUser = await _userRepository.GetByUserCodeAsync(userCode);
        return _currentUser;
    }
}