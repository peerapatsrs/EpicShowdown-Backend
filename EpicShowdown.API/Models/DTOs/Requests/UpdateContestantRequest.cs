using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class UpdateContestantRequest
    {
        [StringLength(100)]
        [Required]
        public required string FieldName { get; set; }

        [Required]
        public required string Value { get; set; }
    }
}