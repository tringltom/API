using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using Application.Validations.ActivityValidation.Challenge;
using AutoMapper;
using DAL;
using DAL.Query;
using Domain;
using FluentValidation;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ChallengeService : IChallengeService
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IUserAccessor _userAccessor;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IEmailManager _emailManager;
        private readonly IValidator<Activity> _activityValidator;
        private readonly IValidator<UserChallengeAnswer> _userChallengeAnswerValidator;

        public ChallengeService(IMapper mapper,
            IUnitOfWork uow,
            IUserAccessor userAccessor,
            IPhotoAccessor photoAccessor,
            IEmailManager emailManager,
            IValidator<Activity> activityValidator,
            IValidator<UserChallengeAnswer> userChallengeAnswerValidator)
        {
            _mapper = mapper;
            _uow = uow;
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
            _emailManager = emailManager;
            _activityValidator = activityValidator;
            _userChallengeAnswerValidator = userChallengeAnswerValidator;
        }

        public async Task<ChallengeEnvelope> GetChallengesForApprovalAsync(QueryObject queryObject)
        {
            var challengesForApproval = await _uow.Activities.GetChallengesForApprovalAsync(queryObject);
            return new ChallengeEnvelope
            {
                Challenges = _mapper.Map<IEnumerable<Activity>, IEnumerable<ChallengeReturn>>(challengesForApproval).ToList(),
                ChallengeCount = await _uow.Activities.CountChallengesForApprovalAsync()
            };
        }

        public async Task<Either<RestError, ChallengeAnswerEnvelope>> GetOwnerChallengeAnswersAsync(int activityId, QueryObject queryObject)
        {
            var activity = await _uow.Activities.GetAsync(activityId);

            var validation = _activityValidator
                .Validate(activity, o => o.IncludeRuleSets(nameof(GetOwnerChallengeAnswersValidator)));

            if (!validation.IsValid)
                return (RestError)validation.Errors.Single().CustomState;

            var userChallengeAnswers = await _uow.UserChallengeAnswers.GetUserChallengeAnswersAsync(activityId, queryObject);

            return new ChallengeAnswerEnvelope
            {
                ChallengeAnswers = _mapper.Map<IEnumerable<UserChallengeAnswer>, IEnumerable<ChallengeAnswerReturn>>(userChallengeAnswers).ToList(),
                ChallengeAnswersCount = await _uow.UserChallengeAnswers.CountChallengeAnswersAsync(activityId)
            };
        }
        public async Task<Either<RestError, Unit>> AnswerToChallengeAsync(int activityId, ChallengeAnswer challengeAnswer)
        {
            var activity = await _uow.Activities.GetAsync(activityId);

            var validation = _activityValidator
                .Validate(activity, o => o.IncludeRuleSets(nameof(AnswerToChallengeValidator)));

            if (!validation.IsValid)
                return (RestError)validation.Errors.Single().CustomState;

            var userId = _userAccessor.GetUserIdFromAccessToken();

            var userChallengeAnswer = new UserChallengeAnswer
            {
                UserId = userId,
                ActivityId = activityId,
                Description = challengeAnswer.Description,
                ChallengeMedias = new List<ChallengeMedia>()
            };

            foreach (var image in challengeAnswer?.Images ?? new IFormFile[0])
            {
                var photoResult = image != null ? await _photoAccessor.AddPhotoAsync(image) : null;
                if (photoResult != null)
                    userChallengeAnswer.ChallengeMedias.Add(new ChallengeMedia() { PublicId = photoResult.PublicId, Url = photoResult.Url });
            }

            var existingAnswerToChallenge = await _uow.UserChallengeAnswers.GetUserChallengeAnswerAsync(userId, activityId);

            if (existingAnswerToChallenge != null)
            {
                existingAnswerToChallenge.Description = challengeAnswer.Description;
                var challengeMedias = await _uow.ChallengeMedias.GetChallengeMedias(existingAnswerToChallenge.Id);

                foreach (var image in challengeMedias ?? new List<ChallengeMedia>())
                {
                    await _photoAccessor.DeletePhotoAsync(image.PublicId);
                }

                existingAnswerToChallenge.ChallengeMedias = userChallengeAnswer.ChallengeMedias;
                _uow.ChallengeMedias.RemoveRange(challengeMedias);
            }
            else
            {
                _uow.UserChallengeAnswers.Add(userChallengeAnswer);
                var user = await _uow.Users.GetAsync(userId);
                await _emailManager.SendChallengeAnsweredEmailAsync(activity.Title, activity.User.Email, user.UserName);
            }

            await _uow.CompleteAsync();

            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> ConfirmChallengeAnswerAsync(int challengeAnswerId)
        {
            var existingAnswerToChallenge = await _uow.UserChallengeAnswers.GetAsync(challengeAnswerId);

            var validation = _userChallengeAnswerValidator
                .Validate(existingAnswerToChallenge, o => o.IncludeRuleSets(nameof(ConfirmChallengeValidator)));

            if (!validation.IsValid)
                return (RestError)validation.Errors.Single().CustomState;

            var confirmedAnswer = await _uow.UserChallengeAnswers.GetConfirmedUserChallengeAnswersAsync(existingAnswerToChallenge.ActivityId);

            if (confirmedAnswer != null)
                confirmedAnswer.Confirmed = false;

            existingAnswerToChallenge.Confirmed = true;

            await _uow.CompleteAsync();

            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> DisapproveChallengeAnswerAsync(int challengeAnswerId)
        {
            var answerToChallenge = await _uow.UserChallengeAnswers.GetAsync(challengeAnswerId);

            if (answerToChallenge == null)
                return new NotFound("Nepostojeći odgovor");

            answerToChallenge.Confirmed = false;

            await _uow.CompleteAsync();

            await _emailManager.SendActivityApprovalEmailAsync(answerToChallenge.Activity.Title, answerToChallenge.Activity.User.Email, true);

            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> ApproveChallengeAnswerAsync(int challengeAnswerId)
        {
            var answerToChallenge = await _uow.UserChallengeAnswers.GetAsync(challengeAnswerId);

            var validation = _userChallengeAnswerValidator
                .Validate(answerToChallenge, o => o.IncludeRuleSets(nameof(ApproveChallengeAnswerValidator)));

            if (!validation.IsValid)
                return (RestError)validation.Errors.Single().CustomState;

            foreach (var media in answerToChallenge.ChallengeMedias)
            {
                answerToChallenge.Activity.ActivityMedias.Add(new ActivityMedia { PublicId = media.PublicId, Url = media.Url });
            }

            answerToChallenge.Activity.Description += $"->Odgovor dat od korisnika {answerToChallenge.User.UserName}:"
                + (!string.IsNullOrEmpty(answerToChallenge.Description) ? $": {answerToChallenge.Description}" : "");

            var userSkill = await _uow.Skills.GetChallengeSkillAsync(answerToChallenge.UserId);
            var xpMultiplier = userSkill != null && userSkill.IsInSecondTree() ? await _uow.SkillXpBonuses.GetSkillMultiplierAsync(userSkill) : 1;

            var challengeXps = await _uow.ActivityReviewXps.GetChallengeXpRewardAsync();

            var reviewXp = 0;

            foreach (var review in answerToChallenge.Activity.UserReviews)
            {
                reviewXp += challengeXps.SingleOrDefault(cx => cx.ReviewTypeId == review.ReviewTypeId).Xp;
            }

            answerToChallenge.Activity.XpReward = reviewXp;

            var xpIncrease = reviewXp * xpMultiplier;

            answerToChallenge.User.CurrentXp += xpIncrease;

            var userChallangeAnswersForDeletion = await _uow.UserChallengeAnswers.GetNotConfirmedUserChallengeAnswersAsync(answerToChallenge.ActivityId);

            foreach (var challangeAnswer in userChallangeAnswersForDeletion)
            {
                foreach (var photo in challangeAnswer.ChallengeMedias)
                {
                    await _photoAccessor.DeletePhotoAsync(photo.PublicId);
                }
            }

            _uow.UserChallengeAnswers.RemoveRange(userChallangeAnswersForDeletion);
            await _uow.CompleteAsync();

            await _emailManager.SendActivityApprovalEmailAsync(answerToChallenge.Activity.Title, answerToChallenge.Activity.User.Email, true);
            await _emailManager.SendChallengeAnswerAcceptedEmailAsync(answerToChallenge.Activity.Title, answerToChallenge.User.Email);

            return Unit.Default;
        }
    }
}
