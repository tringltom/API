﻿using System.Linq;
using Application.Models.User;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    public class UserRangingProfile : Profile
    {
        public UserRangingProfile()
        {
            CreateMap<User, UserRangingGet>()
               .ForMember(d => d.CurrentLevel, o => o.MapFrom(s => s.XpLevelId))
               .ForMember(d => d.NumberOfGoodDeeds, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.GoodDeed).Count()))
               .ForMember(d => d.NumberOfJokes, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Joke).Count()))
               .ForMember(d => d.NumberOfQuotes, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Quote).Count()))
               .ForMember(d => d.NumberOfPuzzles, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Puzzle).Count()))
               .ForMember(d => d.NumberOfHappenings, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Happening).Count()))
               .ForMember(d => d.NumberOfChallenges, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Challenge).Count()));
        }
    }
}