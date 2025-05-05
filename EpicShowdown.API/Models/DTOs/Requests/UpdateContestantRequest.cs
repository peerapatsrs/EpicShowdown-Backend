using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Models.DTOs.Public;

namespace EpicShowdown.API.Models.DTOs.Requests
{
    public class UpdateContestantRequest
    {
        public List<ContestantFieldValuePublic> FieldValues { get; set; } = new List<ContestantFieldValuePublic>();
    }
}