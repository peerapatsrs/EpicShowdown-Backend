using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class UpdateContestantRequest
    {
        [StringLength(100)]
        public string? FieldName { get; set; }

        public string? Value { get; set; }
    }
}