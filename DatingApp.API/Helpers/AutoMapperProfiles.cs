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

            CreateMap<Photo, PhotoReturnDto>();
            CreateMap<UserPhotoDto, Photo>();
            CreateMap<RegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
            .ForMember(m=>m.SenderPhotoUrl, opt => opt.MapFrom(a => a.Sender.Photos.FirstOrDefault(s => s.IsMain).Url))
            .ForMember(m=>m.RecipientPhotoUrl, opt => opt.MapFrom(a => a.Recipient.Photos.FirstOrDefault(s => s.IsMain).Url));
        }

    }
}