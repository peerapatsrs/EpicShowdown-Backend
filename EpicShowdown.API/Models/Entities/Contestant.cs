using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class Contestant : Entity<int>
    {
        [Required]
        [StringLength(100)]
        public required string FieldName { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Value { get; set; }

        [Required]
        public required int ContestId { get; set; }

        [Required]
        [ForeignKey("ContestId")]
        public required virtual Contest Contest { get; set; }

        // Navigation property for received gifts
        public virtual ICollection<ContestantGift> ReceivedGifts { get; set; } = new List<ContestantGift>();
    }
}