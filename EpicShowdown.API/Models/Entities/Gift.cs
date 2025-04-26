using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class Gift : Entity<int>
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public required Guid Code { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public required string ImageUrl { get; set; }

        [Required]
        public required bool IsActive { get; set; } = true;

        public ICollection<ContestGift> ContestGifts { get; set; } = new List<ContestGift>();
    }
}