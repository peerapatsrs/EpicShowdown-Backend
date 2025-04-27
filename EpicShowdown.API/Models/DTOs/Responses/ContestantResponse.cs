using System;
using System.Collections.Generic;

namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class ContestantResponse
    {
        public int Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public Guid ContestCode { get; set; }
        public ICollection<ContestantGiftResponse> ReceivedGifts { get; set; } = new List<ContestantGiftResponse>();
    }
}