using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class CreateContestantRequest
    {
        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}