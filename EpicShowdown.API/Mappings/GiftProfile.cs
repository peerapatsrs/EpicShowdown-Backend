using AutoMapper;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;

namespace EpicShowdown.API.Mappings
{
    public class GiftProfile : Profile
    {
        public GiftProfile()
        {
            CreateMap<Gift, GiftResponse>();
            CreateMap<CreateGiftRequest, Gift>();
            CreateMap<UpdateGiftRequest, Gift>();
        }
    }
}