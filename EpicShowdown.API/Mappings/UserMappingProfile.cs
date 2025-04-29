using AutoMapper;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Models.Entities;

namespace EpicShowdown.API.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, ProfileResponse>();
        }
    }
}