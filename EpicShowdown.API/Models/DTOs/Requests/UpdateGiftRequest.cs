using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class UpdateGiftRequest
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public int Points { get; set; }
    }
}