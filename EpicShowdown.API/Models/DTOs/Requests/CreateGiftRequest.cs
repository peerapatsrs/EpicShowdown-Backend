using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class CreateGiftRequest
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public required string ImageUrl { get; set; }
    }
}