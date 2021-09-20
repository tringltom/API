using AutoMapper;
using Domain.Entities;
using Models.User;

namespace API.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRegister, User>();
        }
    }
}
