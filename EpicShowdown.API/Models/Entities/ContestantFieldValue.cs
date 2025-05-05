using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class ContestantFieldValue : Entity<int>
    {
        [ForeignKey("ContestContestantId")]
        public virtual ContestContestant? ContestContestant { get; set; }

        [Required]
        public required string FieldName { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Value { get; set; }
    }
}