using System;
using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities
{
    public class ContestantGift : Entity<int>
    {
        [Required]
        public required Contestant Contestant { get; set; }
        public int ContestantId { get; set; }

        [Required]
        public required ContestGift ContestGift { get; set; }
        public int ContestGiftId { get; set; }

        public User? GivenBy { get; set; }
        public int? GivenById { get; set; }

        [Required]
        public required DateTime GivenAt { get; set; }

        [StringLength(500)]
        public string? Message { get; set; }
        public static (bool CanGiveGift, string? ErrorMessage) CanGiveGift(Contest contest)
        {
            var now = DateTime.UtcNow;

            // ตรวจสอบว่า Contest ยังเปิดอยู่หรือไม่
            if (!contest.IsActive)
            {
                return (false, "Contest is not active");
            }

            // ตรวจสอบว่าอยู่ในช่วงเวลาของ Contest หรือไม่
            if (now < contest.StartDate)
            {
                return (false, "Contest has not started yet");
            }
            if (now > contest.EndDate)
            {
                return (false, "Contest has ended");
            }

            // ตรวจสอบว่าอยู่ในช่วงเวลาที่สามารถส่งของขวัญได้หรือไม่
            if (contest.GiftStartDate.HasValue && now < contest.GiftStartDate.Value)
            {
                return (false, "Gift giving has not started yet");
            }
            if (contest.GiftEndDate.HasValue && now > contest.GiftEndDate.Value)
            {
                return (false, "Gift giving has ended");
            }

            return (true, null);
        }
    }
}