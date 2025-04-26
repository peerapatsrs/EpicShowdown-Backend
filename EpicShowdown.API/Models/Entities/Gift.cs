using System;
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
        [StringLength(50)]
        public required string Code { get; set; }

        [Required]
        public decimal PricePerPiece { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public required string ImageUrl { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}