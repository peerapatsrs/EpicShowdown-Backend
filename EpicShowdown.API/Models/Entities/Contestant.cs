using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class Contestant : Entity<int>
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Dynamic properties stored as JSON
        [Column(TypeName = "nvarchar(max)")]
        public string AdditionalProperties { get; set; } = "{}";

        public int ContestId { get; set; }
        [ForeignKey("ContestId")]
        public virtual Contest Contest { get; set; } = null!;
    }
}