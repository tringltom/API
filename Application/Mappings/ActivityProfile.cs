﻿using System;
using System.Collections.Generic;
using Application.Activities;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace API.Mappings
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
        }
    }
}