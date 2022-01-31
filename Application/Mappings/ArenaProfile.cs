using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Domain.Entities;
using Models.User;

namespace Application.Mappings
{
    public class ArenaProfile : Profile
    {
        public ArenaProfile()
        {
            CreateMap<User, UserArenaGet>()
               .ForMember(d => d.Username, o => o.MapFrom(s => s.UserName))
               .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
               .ForMember(d => d.CurrentXp, o => o.MapFrom(s => s.CurrentXp))
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
