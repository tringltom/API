using System;
using System.Collections.Generic;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace Application.Mappings
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<ActivityCreate, PendingActivity>()
                .BeforeMap((s, d) => d.DateCreated = DateTimeOffset.Now)
                .BeforeMap((s, d) => d.PendingActivityMedias = new List<PendingActivityMedia>())
                .ForMember(d => d.ActivityTypeId, o => o.MapFrom(s => s.Type))
                .ForMember(d => d.User, o => o.MapFrom<UserResolver>());

            CreateMap<FavoriteActivityCreate, UserFavoriteActivity>();

            CreateMap<FavoriteActivityRemove, UserFavoriteActivity>();
        }
    }
}
