﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Models.Activity;

namespace Application.Services
{
    public class ActivityReviewService : IActivityReviewService
    {
        private readonly IMapper _mapper;
        private readonly IUserReviewRepository _userReviewRepository;

        public ActivityReviewService(IMapper mapper, IUserReviewRepository userReviewRepository)
        {
            _mapper = mapper;
            _userReviewRepository = userReviewRepository;
        }

        public async Task<UserReview> GetUserReviewByActivityAndUserIds(int activityId, int userId)
        {
            return await _userReviewRepository.GetUserReviewByActivityAndUserIdAsync(activityId, userId);
        }

        public async Task UpdateReviewActivityAsync(ActivityReview activityReview)
        {
            var review = _mapper.Map<UserReview>(activityReview);
            try
            {
                await _userReviewRepository.UpdateUserActivityReviewAsync(review);
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.InternalServerError, new { Activity = "Neuspešna izmena ocene aktivnosti." });
            }
        }

        public async Task AddReviewActivityAsync(ActivityReview activityReview)
        {
            var review = _mapper.Map<UserReview>(activityReview);
            try
            {
                await _userReviewRepository.ReviewUserActivityAsync(review);
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.InternalServerError, new { Activity = "Neuspešna ocena aktivnosti." });
            }
        }

        public async Task<IList<ActivityReviewedByUser>> GetAllReviewsByUserId(int userId)
        {
            var userReviews = await _userReviewRepository.GetAllUserReviewsAsQeuriable().Where(x => x.UserId == userId).ToListAsync();

            return _mapper.Map<List<ActivityReviewedByUser>>(userReviews);
            // List<UserReview>,
        }
    }
}
