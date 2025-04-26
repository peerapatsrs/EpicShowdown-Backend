using System;

namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class ContestGiftResponse
    {
        public int Id { get; set; }
        public Guid GiftCode { get; set; }
        public string GiftName { get; set; } = string.Empty;
        public string GiftImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }
    }
}