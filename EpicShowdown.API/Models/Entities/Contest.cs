using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class Contest : Entity<int>
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(50)]
        public required Guid ContestCode { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public required DateTime StartDate { get; set; }

        [Required]
        public required DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public required User CreatedBy { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal FeePercentage { get; set; } = 7.0m;

        public int? DisplayTemplateId { get; set; }
        public virtual DisplayTemplate? DisplayTemplate { get; set; }

        // Navigation properties
        public virtual ICollection<Contestant> Contestants { get; set; } = new List<Contestant>();
        public virtual ICollection<ContestantField> Fields { get; set; } = new List<ContestantField>();
        public ICollection<ContestGift> ContestGifts { get; set; } = new List<ContestGift>();
    }
}