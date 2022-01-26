using System;
using System.Collections.Generic;
using AutoMapper;
using Domain.Entities;
using Models;
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

            CreateMap<PendingActivity, Activity>()
                .BeforeMap((s, d) => d.DateApproved = DateTimeOffset.Now)
                .ForMember(d => d.ActivityMedias, o => o.MapFrom(s => s.PendingActivityMedias))
                .ForMember(d => d.Id, o => o.MapFrom(src => 0));

            CreateMap<PendingActivity, PendingActivityReturn>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.ActivityTypeId))
                .ForMember(d => d.Photos, o => o.MapFrom(s => s.PendingActivityMedias));

            CreateMap<Activity, ApprovedActivityReturn>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.ActivityTypeId))
                .ForMember(d => d.Photos, o => o.MapFrom(s => s.ActivityMedias));

            CreateMap<PendingActivityMedia, ActivityMedia>()
                .ForMember(d => d.Activity, o => o.MapFrom(s => s.ActivityPending))
                .ForMember(d => d.Id, o => o.MapFrom(src => 0));

            CreateMap<PendingActivityMedia, Photo>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.PublicId));

            CreateMap<FavoriteActivityCreate, UserFavoriteActivity>();

            CreateMap<FavoriteActivityRemove, UserFavoriteActivity>();

            CreateMap<ActivityReview, UserReview>();

            CreateMap<UserReview, ActivityReviewedByUser>();

            CreateMap<UserFavoriteActivity, FavoriteActivityReturn>();
        }
    }
}
