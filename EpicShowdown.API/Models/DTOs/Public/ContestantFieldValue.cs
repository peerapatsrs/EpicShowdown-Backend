using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Public
{
    public class ContestantFieldValuePublic
    {
        [Required]
        public string FieldName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}