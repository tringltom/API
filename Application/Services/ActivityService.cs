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
using Domain;
using LanguageExt;

namespace Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly IEmailManager _emailManager;
        private readonly IUnitOfWork _uow;

        public ActivityService(IUserAccessor userAccessor, IMapper mapper, IEmailManager emailManager, IUnitOfWork uow)
        {
            _userAccessor = userAccessor;
            _mapper = mapper;
            _emailManager = emailManager;
            _uow = uow;
        }

        public async Task<ApprovedActivityReturn> GetActivityAsync(int id)
        {
            var activity = await _uow.Activities.GetAsync(id);
            return _mapper.Map<ApprovedActivityReturn>(activity);
        }

        public async Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(int? limit, int? offset)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();

            var activities = await _uow.Activities.GetOrderedActivitiesFromOtherUsersAsync(limit, offset, userId);

            return new ApprovedActivityEnvelope
            {
                Activities = _mapper.Map<IEnumerable<Activity>, IEnumerable<ApprovedActivityReturn>>(activities).ToList(),
                ActivityCount = await _uow.Activities.CountOtherUsersActivitiesAsync(userId)
            };
        }

        public async Task<Either<RestError, int>> AnswerToPuzzleAsync(int id, PuzzleAnswer puzzleAnswer)
        {
            var activity = await _uow.Activities.GetAsync(id);

            if (activity == null)
                return new NotFound("Aktivnost nije pronadjena");

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

            var newUserAnswer = new UserPuzzleAnswer { ActivityId = id, UserId = userId };
            _uow.UserPuzzleAnswers.Add(newUserAnswer);

            await _uow.CompleteAsync();
            return xpIncrease;
        }

        public async Task<Either<RestError, ApprovedActivityReturn>> ApprovePendingActivity(int id)
        {
            var pendingActivity = await _uow.PendingActivities.GetAsync(id);

            if (pendingActivity == null)
                return new NotFound("Aktivnost nije pronadjena");

            var activity = _mapper.Map<Activity>(pendingActivity);

            _uow.Activities.Add(activity);
            _uow.PendingActivities.Remove(pendingActivity);
            await _uow.CompleteAsync();

            await _emailManager.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, true);

            return _mapper.Map<ApprovedActivityReturn>(activity);
        }
    }
}
