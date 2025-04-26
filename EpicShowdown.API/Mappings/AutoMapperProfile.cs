using AutoMapper;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;

namespace EpicShowdown.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Contest Mappings
            CreateMap<Contest, ContestResponse>();
            CreateMap<CreateContestRequest, Contest>();
            CreateMap<UpdateContestRequest, Contest>();

            // Contestant Mappings
            CreateMap<Contestant, ContestantResponse>()
                .ForMember(dest => dest.ContestCode, opt => opt.MapFrom(src => src.Contest.ContestCode));
            CreateMap<CreateContestantRequest, Contestant>();
            CreateMap<UpdateContestantRequest, Contestant>();

            // ContestantField Mappings
            CreateMap<ContestantField, ContestantFieldResponse>();
            CreateMap<CreateContestantFieldRequest, ContestantField>();
            CreateMap<UpdateContestantFieldRequest, ContestantField>();

            // Gift Mappings
            CreateMap<Gift, GiftResponse>();
            CreateMap<CreateGiftRequest, Gift>();
            CreateMap<UpdateGiftRequest, Gift>();
        }
    }
}