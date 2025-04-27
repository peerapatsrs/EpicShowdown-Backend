using EpicShowdown.API.Models.Entities;

namespace EpicShowdown.API.Models.DTOs.Responses;

public class PassKeyAuthenticationResponse
{
    public bool Success { get; set; }
    public User? User { get; set; }
}