namespace EpicShowdown.API.Models.DTOs.Requests;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RefreshTokenRequest
{
    public required string RefreshToken { get; set; }
}

public class RevokeTokenRequest
{
    public required string RefreshToken { get; set; }
    public string? Reason { get; set; }
}