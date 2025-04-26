using System;
using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class CreateContestGiftRequest
    {
        [Required]
        public Guid GiftCode { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Points { get; set; }

        public bool IsActive { get; set; } = true;
    }
}