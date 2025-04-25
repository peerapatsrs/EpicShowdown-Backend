using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Models.Enums;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class CreateContestantFieldRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ContestantFieldType Type { get; set; }

        public bool IsRequired { get; set; }

        public string? DefaultValue { get; set; }

        public string? ValidationRules { get; set; }

        public int Order { get; set; }
    }
}