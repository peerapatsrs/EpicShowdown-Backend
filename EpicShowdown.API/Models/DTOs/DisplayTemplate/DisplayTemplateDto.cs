using System.ComponentModel.DataAnnotations;

namespace EpicShowdown.API.Models.DTOs.DisplayTemplate
{
    public class CreateDisplayTemplateRequest
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public required string Content { get; set; }
    }

    public class UpdateDisplayTemplateRequest
    {
        [Required]
        public required Guid Code { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public required string Content { get; set; }
    }

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