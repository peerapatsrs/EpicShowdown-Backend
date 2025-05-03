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
            // User Mappings
            CreateMap<User, ProfileResponse>();

            // Contest Mappings
            CreateMap<Contest, ContestResponse>()
                .ForMember(dest => dest.DisplayTemplateCode,
                    opt => opt.MapFrom(src => src.DisplayTemplate != null ? src.DisplayTemplate.Code : (Guid?)null));
            CreateMap<CreateContestRequest, Contest>()
                .ForMember(dest => dest.DisplayTemplate, opt => opt.Ignore());
            CreateMap<UpdateContestRequest, Contest>()
                .ForMember(dest => dest.DisplayTemplate, opt => opt.Ignore());

            // Contestant Mappings
            CreateMap<Contestant, ContestantResponse>()
                .ForMember(dest => dest.ContestCode, opt => opt.MapFrom(src => src.Contest.ContestCode));
            CreateMap<CreateContestantRequest, Contestant>();
            CreateMap<UpdateContestantRequest, Contestant>();

            // ContestantGift Mappings
            CreateMap<ContestantGift, ContestantGiftResponse>()
                .ForMember(dest => dest.GiftName, opt => opt.MapFrom(src => src.ContestGift.Gift.Name))
                .ForMember(dest => dest.GiftPrice, opt => opt.MapFrom(src => src.ContestGift.Price))
                .ForMember(dest => dest.GiftPoints, opt => opt.MapFrom(src => src.ContestGift.Points))
                .ForMember(dest => dest.GivenByName, opt => opt.MapFrom(src => src.GivenBy != null ? $"{src.GivenBy.FirstName} {src.GivenBy.LastName}" : string.Empty));

            // ContestantField Mappings
            CreateMap<ContestantField, ContestantFieldResponse>();
            CreateMap<CreateContestantFieldRequest, ContestantField>();
            CreateMap<UpdateContestantFieldRequest, ContestantField>();

            // Gift Mappings
            CreateMap<Gift, GiftResponse>();
            CreateMap<CreateGiftRequest, Gift>();
            CreateMap<UpdateGiftRequest, Gift>();

            // DisplayTemplate Mappings
            CreateMap<DisplayTemplate, DisplayTemplateResponse>();
            CreateMap<CreateDisplayTemplateRequest, DisplayTemplate>();
            CreateMap<UpdateDisplayTemplateRequest, DisplayTemplate>();
        }
    }
}