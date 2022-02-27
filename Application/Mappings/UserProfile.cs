using System;
using System.Linq;
using Application.Models;
using Application.Models.User;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserRangingGet>()
               .ForMember(d => d.CurrentLevel, o => o.MapFrom(s => s.XpLevelId))
               .ForMember(d => d.NumberOfGoodDeeds, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.GoodDeed).Count()))
               .ForMember(d => d.NumberOfJokes, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Joke).Count()))
               .ForMember(d => d.NumberOfQuotes, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Quote).Count()))
               .ForMember(d => d.NumberOfPuzzles, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Puzzle).Count()))
               .ForMember(d => d.NumberOfHappenings, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Happening).Count()))
               .ForMember(d => d.NumberOfChallenges, o => o.MapFrom(s => s.Activities.Where(x => x.ActivityTypeId == ActivityTypeId.Challenge).Count()))
               .ForMember(d => d.Image, o =>
               {
                   o.PreCondition(s => s.ImageApproved);
                   o.MapFrom(s => new Photo() { Id = s.ImagePublicId, Url = s.ImageUrl });
               });

            CreateMap<User, UserBaseResponse>()
               .ForMember(d => d.CurrentLevel, o => o.MapFrom(s => s.XpLevelId))
               .ForMember(d => d.IsDiceRollAllowed, o => o.MapFrom(s => s.LastRollDate == null || (DateTimeOffset.Now - s.LastRollDate) >= TimeSpan.FromDays(1)))
               .ForMember(d => d.Image, o =>
               {
                   o.PreCondition(s => s.ImageApproved);
                   o.MapFrom(s => new Photo() { Id = s.ImagePublicId, Url = s.ImageUrl });
               });

            CreateMap<User, UserImageResponse>()
               .ForMember(d => d.Image, o => { o.MapFrom(s => new Photo() { Id = s.ImagePublicId, Url = s.ImageUrl }); });
        }
    }
}
