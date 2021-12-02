using Application.Repositories;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace Application.Activities;

public class UserResolver : IValueResolver<ActivityCreate, PendingActivity, User>
{
    private readonly IUserRepository _userRepository;

    public UserResolver(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User Resolve(ActivityCreate source, PendingActivity destination, User destMember, ResolutionContext context)
    {
        return _userRepository.GetUserByID().Result;
    }
}

