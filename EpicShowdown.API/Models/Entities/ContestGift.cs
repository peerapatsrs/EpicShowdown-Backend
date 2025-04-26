using System;
using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class ContestGift : Entity<int>
    {
        [Required]
        public required Contest Contest { get; set; }
        public int ContestId { get; set; }

        [Required]
        public required Gift Gift { get; set; }
        public int GiftId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Points { get; set; }

        public bool IsActive { get; set; } = true;
    }
}