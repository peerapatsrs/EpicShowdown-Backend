using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class Contestant : Entity<int>
    {
        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Value { get; set; } = string.Empty;

        public int ContestId { get; set; }
        [ForeignKey("ContestId")]
        public virtual Contest Contest { get; set; } = null!;
    }
}