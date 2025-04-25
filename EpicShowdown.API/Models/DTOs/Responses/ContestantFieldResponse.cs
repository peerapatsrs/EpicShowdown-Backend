using System;
using EpicShowdown.API.Models.Enums;

namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class ContestantFieldResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ContestantFieldType Type { get; set; }
        public bool IsRequired { get; set; }
        public string? DefaultValue { get; set; }
        public string? ValidationRules { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}