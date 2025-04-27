using System;
using System.Collections.Generic;
using EpicShowdown.API.Models.DTOs.Responses;

namespace EpicShowdown.API.Models.DTOs.Responses
{
    public class ContestResponse
    {
        public Guid ContestCode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? GiftStartDate { get; set; }
        public DateTime? GiftEndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? DisplayTemplateCode { get; set; }
        public ICollection<ContestantResponse> Contestants { get; set; } = new List<ContestantResponse>();
        public ICollection<ContestantFieldResponse> Fields { get; set; } = new List<ContestantFieldResponse>();
    }
}