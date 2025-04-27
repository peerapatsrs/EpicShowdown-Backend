using System;

namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class ContestantGiftResponse
    {
        public int Id { get; set; }
        public int ContestGiftId { get; set; }
        public string GiftName { get; set; } = string.Empty;
        public decimal GiftPrice { get; set; }
        public int GiftPoints { get; set; }
        public string GivenByName { get; set; } = string.Empty;
        public DateTime GivenAt { get; set; }
        public string? Message { get; set; }
    }
}