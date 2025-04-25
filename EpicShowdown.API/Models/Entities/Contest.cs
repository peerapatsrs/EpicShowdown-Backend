using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class Contest : Entity<int>
    {
        public Guid ContestCode { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public required User CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<Contestant> Contestants { get; set; } = new List<Contestant>();
        public virtual ICollection<ContestantField> Fields { get; set; } = new List<ContestantField>();
    }
}