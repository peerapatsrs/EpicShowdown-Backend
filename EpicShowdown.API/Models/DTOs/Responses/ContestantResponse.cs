using System;
using System.Collections.Generic;
using EpicShowdown.API.Models.DTOs.Public;

namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class ContestantResponse
    {
        public int Id { get; set; }
        public List<ContestantFieldValuePublic> FieldValues { get; set; } = new List<ContestantFieldValuePublic>();
        public Guid ContestCode { get; set; }
    }
}