using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class UpdateGiftRequest
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        public bool IsActive { get; set; }
    }
}