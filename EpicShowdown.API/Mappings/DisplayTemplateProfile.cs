using AutoMapper;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Models.Entities;

namespace EpicShowdown.API.Mappings
{
    public class DisplayTemplateProfile : Profile
    {
        public DisplayTemplateProfile()
        {
            CreateMap<DisplayTemplate, DisplayTemplateResponse>();
            CreateMap<CreateDisplayTemplateRequest, DisplayTemplate>();
            CreateMap<UpdateDisplayTemplateRequest, DisplayTemplate>()
                .ForMember(dest => dest.Code, opt => opt.Ignore());
        }
    }
}