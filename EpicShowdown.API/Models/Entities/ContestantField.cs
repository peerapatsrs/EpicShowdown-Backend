using System;
using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Data.Base;
using EpicShowdown.API.Models.Enums;

namespace EpicShowdown.API.Models.Entities
{
    public class ContestantField : Entity<int>
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(500)]
        public required string Description { get; set; }

        [Required]
        public required ContestantFieldType Type { get; set; }

        [Required]
        public required bool IsRequired { get; set; }

        public string? DefaultValue { get; set; }

        public string? ValidationRules { get; set; }

        [Required]
        public required int Order { get; set; }

        [Required]
        public required int ContestId { get; set; }

        [Required]
        public required virtual Contest Contest { get; set; }
    }
}