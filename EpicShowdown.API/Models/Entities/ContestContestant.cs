using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EpicShowdown.API.Data.Base;
using Microsoft.VisualBasic;

namespace EpicShowdown.API.Models.Entities
{
    public class ContestContestant : Entity<int>
    {
        [Required]
        [ForeignKey("ContestId")]
        public required virtual Contest Contest { get; set; }
        public int ContestId { get; set; }
        public virtual ICollection<ContestantFieldValue> ContestantFieldValues { get; set; } = new List<ContestantFieldValue>();

        // Navigation property for received gifts
        public virtual ICollection<ContestantGift> ReceivedGifts { get; set; } = new List<ContestantGift>();
    }
}