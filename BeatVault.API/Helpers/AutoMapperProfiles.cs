using AutoMapper;
using BeatVault.API.DTOs;
using BeatVault.API.Entities;

namespace BeatVault.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Map Beat -> BeatDto
            CreateMap<Beat, BeatDto>()
                .ForMember(dest => dest.ProducerName, opt => opt.MapFrom(src => src.Producer.Username));

            // Map RegisterDto -> User
            CreateMap<RegisterDto, User>();
        }
    }
}