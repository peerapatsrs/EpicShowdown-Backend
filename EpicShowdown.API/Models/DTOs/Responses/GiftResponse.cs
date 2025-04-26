namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class GiftResponse
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}