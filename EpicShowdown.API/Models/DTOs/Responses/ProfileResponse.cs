using System;

namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class ProfileResponse
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}