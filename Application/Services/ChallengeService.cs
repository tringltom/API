using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using DAL.Query;
using Domain;
using AutoMapper;
using LanguageExt;
using Application.Errors;
using DAL;
using System.Linq;
using Application.InfrastructureInterfaces.Security;
using Microsoft.AspNetCore.Http;
using Application.InfrastructureInterfaces;

namespace Application.Services
{
    public class ChallengeService : IChallengeService
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IUserAccessor _userAccessor;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IEmailManager _emailManager;

        public ChallengeService(IMapper mapper, IUnitOfWork uow, IUserAccessor userAccessor, IPhotoAccessor photoAccessor, IEmailManager emailManager)
        {
            _mapper = mapper;
            _uow = uow;
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
            _emailManager = emailManager;
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

            if (activity == null)
                return new NotFound("Aktivnost nije pronađena");

            if (activity.ActivityTypeId != ActivityTypeId.Challenge)
                return new BadRequest("Aktivnost nije izazov");

            if (activity.XpReward != null)
                return new BadRequest("Izazov je rešen");

            var userId = _userAccessor.GetUserIdFromAccessToken();

            if (activity.User.Id != userId)
                return new BadRequest("Niste kreirali ovaj izazov");

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

            if (activity == null)
                return new NotFound("Aktivnost nije pronađena");

            var userId = _userAccessor.GetUserIdFromAccessToken();

            if (activity.User.Id == userId)
                return new BadRequest("Ne možete odgovoriti na svoj izazov");

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

            if (existingAnswerToChallenge == null)
                return new NotFound("Nepostojeći odgovor");

            if (existingAnswerToChallenge.Confirmed)
                return new BadRequest("Ovaj odgovor ste već odabrali");

            var userId = _userAccessor.GetUserIdFromAccessToken();

            if (userId != existingAnswerToChallenge.Activity.User.Id)
                return new BadRequest("Ne možete odabrati odgovor za izazov koji niste kreirali");

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

            if (answerToChallenge == null)
                return new NotFound("Nepostojeći odgovor");

            if (!answerToChallenge.Confirmed)
                return new BadRequest("Ovaj odgovor nije odabran");

            if (answerToChallenge.Activity.XpReward != null)
                return new BadRequest("Izazov je rešen");

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
