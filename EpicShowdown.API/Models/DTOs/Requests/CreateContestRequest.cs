using System;
using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class CreateContestRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime? GiftStartDate { get; set; }

        public DateTime? GiftEndDate { get; set; }

        public Guid? DisplayTemplateCode { get; set; }
    }
}