using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>()
            .ForMember(dest => dest.PhotoUrl, opt => opt
               .MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt
               .MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User, UserDetailsDto>()
             .ForMember(dest => dest.PhotoUrl, opt => opt
               .MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
               .ForMember(dest => dest.Age, opt => opt
               .MapFrom(src => src.DateOfBirth.CalculateAge()));
               
            CreateMap<Photo, PhotoDetailsDto>();

            CreateMap<UserUpdateDto, User>();
            
        }

    }
}