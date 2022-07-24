using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using DAL.Query;
using Domain;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly IEmailManager _emailManager;
        private readonly IUnitOfWork _uow;
        private readonly IPhotoAccessor _photoAccessor;

        public ActivityService(IUserAccessor userAccessor, IMapper mapper, IEmailManager emailManager, IUnitOfWork uow, IPhotoAccessor photoAccessor)
        {
            _userAccessor = userAccessor;
            _mapper = mapper;
            _emailManager = emailManager;
            _uow = uow;
            _photoAccessor = photoAccessor;
        }

        public async Task<ApprovedActivityReturn> GetActivityAsync(int id)
        {
            var activity = await _uow.Activities.GetAsync(id);
            return _mapper.Map<ApprovedActivityReturn>(activity);
        }

        public async Task<ActivitiesFromOtherUserEnvelope> GetActivitiesFromOtherUsersAsync(ActivityQuery activityQuery)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();

            var activities = await _uow.Activities.GetOrderedActivitiesFromOtherUsersAsync(activityQuery, userId);

            return new ActivitiesFromOtherUserEnvelope
            {
                Activities = _mapper.Map<IEnumerable<Activity>, IEnumerable<OtherUserActivityReturn>>(activities).ToList(),
                ActivityCount = await _uow.Activities.CountOtherUsersActivitiesAsync(userId, activityQuery)
            };
        }

        public async Task<ApprovedActivityEnvelope> GetApprovedActivitiesCreatedByUserAsync(int userId, ActivityQuery activityQuery)
        {
            var activities = await _uow.Activities.GetActivitiesCreatedByUser(userId, activityQuery);

            return new ApprovedActivityEnvelope
            {
                Activities = _mapper.Map<IEnumerable<Activity>, IEnumerable<ApprovedActivityReturn>>(activities).ToList(),
                ActivityCount = await _uow.Activities.CountActivitiesCreatedByUser(userId),
            };
        }

        public async Task<Either<RestError, int>> AnswerToPuzzleAsync(int id, PuzzleAnswer puzzleAnswer)
        {
            var activity = await _uow.Activities.GetAsync(id);

            if (activity == null)
                return new NotFound("Aktivnost nije pronađena");

            if (activity.ActivityTypeId != ActivityTypeId.Puzzle)
                return new BadRequest("Aktivnost nije zagonetka");

            if (!string.Equals(activity.Answer.Trim(), puzzleAnswer.Answer.Trim(), StringComparison.OrdinalIgnoreCase))
                return new BadRequest("Netačan odgovor");

            var userId = _userAccessor.GetUserIdFromAccessToken();

            if (activity.User.Id == userId)
                return new BadRequest("Ne možete odgovarati na svoje zagonetke");

            var userAnswer = await _uow.UserPuzzleAnswers.GetUserPuzzleAnswerAsync(userId, id);

            if (userAnswer != null)
                return new BadRequest("Već ste dali tačan odgovor");

            var userSkill = await _uow.Skills.GetPuzzleSkillAsync(userId);
            var xpMultiplier = userSkill != null && userSkill.IsInSecondTree() ? await _uow.SkillXpBonuses.GetSkillMultiplierAsync(userSkill) : 1;
            var user = await _uow.Users.GetAsync(userId);

            if (activity.XpReward == null)
            {
                var xpReward = 100 + 5 * (DateTimeOffset.Now - activity.DateApproved).Days;
                activity.XpReward = xpReward;
            }

            var xpIncrease = (int)activity.XpReward * xpMultiplier;
            user.CurrentXp += xpIncrease;

            _uow.UserPuzzleAnswers.Add(new UserPuzzleAnswer { ActivityId = id, UserId = userId });

            await _uow.CompleteAsync();
            await _emailManager.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName);
            return xpIncrease;
        }

        public async Task<Either<RestError, ApprovedActivityReturn>> ApprovePendingActivity(int id)
        {
            var pendingActivity = await _uow.PendingActivities.GetAsync(id);

            if (pendingActivity == null)
                return new NotFound("Aktivnost nije pronađena");

            var activity = _mapper.Map<Activity>(pendingActivity);

            _uow.Activities.Add(activity);
            _uow.PendingActivities.Remove(pendingActivity);
            await _uow.CompleteAsync();

            await _emailManager.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, true);

            return _mapper.Map<ApprovedActivityReturn>(activity);
        }
    }
}
