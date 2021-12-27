using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Entities;
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

        public async Task ReviewActivityAsync(ActivityReview activityReview)
        {
            var activity = _mapper.Map<UserReview>(activityReview);

            var existingReview = await _userReviewRepository.GetUserReviewByActivityAndUserIdAsync(activity.ActivityId, activity.UserId);

            if (existingReview != null)
            {
                //recalculate exp
                try
                {
                    await _userReviewRepository.UpdateUserActivityReviewAsync(activity);
                }
                catch (Exception)
                {
                    throw new RestException(HttpStatusCode.Conflict, new { Greska = "Greška pri izmeni ocene." });
                }
                return;
            }

            // add exp
            try
            {
                await _userReviewRepository.ReviewUserActivityAsync(activity);
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.Conflict, new { Greska = "Greška pri unosu ocene." });
            }
        }
    }
}
