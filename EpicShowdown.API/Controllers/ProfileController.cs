using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Repositories;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Services;

namespace EpicShowdown.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public ProfileController(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _currentUserService.GetCurrentUser();
        if (user == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.DateOfBirth,
            user.PhoneNumber,
            user.Address
        });
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var user = await _currentUserService.GetCurrentUser();
        if (user == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DateOfBirth = request.DateOfBirth;
        user.PhoneNumber = request.PhoneNumber;
        user.Address = request.Address;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return Ok(new { message = "Profile updated successfully" });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var user = await _currentUserService.GetCurrentUser();
        if (user == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        if (!user.VerifyPassword(request.CurrentPassword))
        {
            return BadRequest(new { message = "Current password is incorrect" });
        }

        user.SetPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return Ok(new { message = "Password changed successfully" });
    }
}