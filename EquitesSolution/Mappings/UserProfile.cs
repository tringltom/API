using API.DTOs.User;
using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace API.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserForRegistrationRequestDto, User>();
            CreateMap<UserBaseServiceResponse, UserBaseResponseDto>();
        }
    }
}
