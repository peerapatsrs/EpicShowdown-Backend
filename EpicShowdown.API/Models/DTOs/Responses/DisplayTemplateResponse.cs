using System;

namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class DisplayTemplateResponse
    {
        public required Guid Code { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Content { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}