using System;
using System.Collections.Generic;
using System.Linq;
using Application.Models;
using Application.Models.Activity;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<ActivityCreate, PendingActivity>()
                .BeforeMap((s, d) => d.DateCreated = DateTimeOffset.Now)
                .BeforeMap((s, d) => d.PendingActivityMedias = new List<PendingActivityMedia>())
                .ForMember(d => d.ActivityTypeId, o => o.MapFrom(s => s.Type));

            CreateMap<PendingActivity, ActivityCreate>()
                .ForMember(d => d.Type, o => o.MapFrom(s => s.ActivityTypeId))
                .ForMember(d => d.Urls, o => o.MapFrom(s => s.PendingActivityMedias.Select(pa => pa.Url)));

            CreateMap<PendingActivity, Activity>()
                .BeforeMap((s, d) => d.DateApproved = DateTimeOffset.Now)
                .ForMember(d => d.ActivityMedias, o => o.MapFrom(s => s.PendingActivityMedias))
                .ForMember(d => d.Id, o => o.MapFrom(src => 0));

            CreateMap<PendingActivity, PendingActivityReturn>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.ActivityTypeId))
                .ForMember(d => d.Photos, o => o.MapFrom(s => s.PendingActivityMedias));

            CreateMap<PendingActivity, PendingActivityForUserReturn>()
                .ForMember(d => d.Type, o => o.MapFrom(s => s.ActivityTypeId));

            CreateMap<Activity, ApprovedActivityReturn>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.ActivityTypeId))
                .ForMember(d => d.Photos, o => o.MapFrom(s => s.ActivityMedias))
                .ForMember(d => d.NumberOfAttendees, o => o.MapFrom(s => s.UserAttendances != null ? s.UserAttendances.Count : 0))
                .ForMember(d => d.IsUserAttending, o => o.MapFrom<AtendeeResolver>())
                .ForMember(d => d.IsHeld, o => o.MapFrom(s => s.EndDate < DateTimeOffset.Now))
                .ForMember(d => d.IsHost, o => o.MapFrom<HostResolver>());

            CreateMap<Activity, HappeningReturn>()
               .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName))
               .ForMember(d => d.Type, o => o.MapFrom(s => s.ActivityTypeId))
               .ForMember(d => d.Photos, o => o.MapFrom(s => s.ActivityMedias))
               .ForMember(d => d.HappeningPhotos, o => o.MapFrom(s => s.HappeningMedias));

            CreateMap<PendingActivityMedia, ActivityMedia>()
                .ForMember(d => d.Activity, o => o.MapFrom(s => s.ActivityPending))
                .ForMember(d => d.Id, o => o.MapFrom(src => 0));

            CreateMap<ActivityMedia, Photo>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.PublicId));

            CreateMap<PendingActivityMedia, Photo>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.PublicId));

            CreateMap<HappeningMedia, Photo>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.PublicId));

            CreateMap<ActivityReview, UserReview>();

            CreateMap<UserReview, UserReviewedActivity>();

            CreateMap<UserFavoriteActivity, FavoriteActivityIdReturn>();

            CreateMap<UserFavoriteActivity, UserFavoriteActivityReturn>();
        }
    }
}
