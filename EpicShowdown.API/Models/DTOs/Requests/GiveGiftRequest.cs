using System;
using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class GiveGiftRequest
    {
        [Required]
        public Guid GiftCode { get; set; }

        [StringLength(500)]
        public string? Message { get; set; }
    }
}