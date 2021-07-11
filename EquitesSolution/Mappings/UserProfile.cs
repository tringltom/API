using API.DTOs.User;
using AutoMapper;
using Domain.Entities;

namespace API.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserForRegistrationRequestDto, User>();
        }
    }
}
