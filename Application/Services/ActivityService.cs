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

        public async Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(ActivityQuery activityQuery)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();

            var activities = await _uow.Activities.GetOrderedActivitiesFromOtherUsersAsync(activityQuery, userId);

            return new ApprovedActivityEnvelope
            {
                Activities = _mapper.Map<IEnumerable<Activity>, IEnumerable<ApprovedActivityReturn>>(activities).ToList(),
                ActivityCount = await _uow.Activities.CountOtherUsersActivitiesAsync(userId, activityQuery)
            };
        }

        public async Task<HappeningEnvelope> GetHappeningsForApprovalAsync(QueryObject queryObject)
        {
            var happeningsForApproval = await _uow.Activities.GetHappeningsForApprovalAsync(queryObject);

            return new HappeningEnvelope
            {
                Happenings = _mapper.Map<IEnumerable<Activity>, IEnumerable<HappeningReturn>>(happeningsForApproval).ToList(),
                HappeningCount = await _uow.Activities.CountHappeningsForApprovalAsync()
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

            _uow.UserPuzzleAnswers.Add(new UserPuzzleAnswer { ActivityId = id, UserId = userId });

            await _uow.CompleteAsync();
            await _emailManager.SendPuzzleAnsweredAsync(activity.Title, activity.User.Email, user.UserName);
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

        public async Task<Either<RestError, Unit>> AttendToHappeningAsync(int id, bool attend)
        {
            var activity = await _uow.Activities.GetAsync(id);

            if (activity == null)
                return new NotFound("Aktivnost nije pronađena");

            if (activity.ActivityTypeId != ActivityTypeId.Happening)
                return new BadRequest("Aktivnost nije Događaj");

            if (activity.EndDate < DateTimeOffset.Now)
                return new BadRequest("Događaj se već održao");

            var userId = _userAccessor.GetUserIdFromAccessToken();

            if (activity.User.Id == userId)
                return new BadRequest("Ne možete reagovati na vaš događaj");

            var userAttendence = activity.UserAttendances.Where(ua => ua.UserId == userId).SingleOrDefault();

            if (attend && userAttendence != null || !attend && userAttendence == null)
                return new BadRequest("Već ste reagovali na događaj");

            if (attend)
                _uow.UserAttendaces.Add(new UserAttendance { UserId = userId, ActivityId = id, Confirmed = false });
            else
                _uow.UserAttendaces.Remove(userAttendence);

            await _uow.CompleteAsync();

            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> ConfirmAttendenceToHappeningAsync(int id)
        {
            var activity = await _uow.Activities.GetAsync(id);

            if (activity == null)
                return new NotFound("Aktivnost nije pronađena");

            if (activity.ActivityTypeId != ActivityTypeId.Happening)
                return new BadRequest("Aktivnost nije Događaj");

            if (activity.EndDate < DateTimeOffset.Now)
                return new BadRequest("Događaj se već održao");

            if (activity.StartDate > DateTimeOffset.Now)
                return new BadRequest("Događaj još nije počeo");

            var userId = _userAccessor.GetUserIdFromAccessToken();

            if (activity.User.Id == userId)
                return new BadRequest("Ne možete potvrditi dolazak na vaš događaj");

            var userAttendence = activity.UserAttendances.Where(ua => ua.UserId == userId).SingleOrDefault();

            if (userAttendence == null)
                _uow.UserAttendaces.Add(new UserAttendance { UserId = userId, ActivityId = id, Confirmed = true });
            else
                userAttendence.Confirmed = true;

            await _uow.CompleteAsync();

            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> CompleteHappeningAsync(int id, HappeningUpdate happeningUpdate)
        {
            var activity = await _uow.Activities.GetAsync(id);

            if (activity == null)
                return new NotFound("Aktivnost nije pronađena");

            if (activity.ActivityTypeId != ActivityTypeId.Happening)
                return new BadRequest("Aktivnost nije Događaj");

            if (activity.EndDate > DateTimeOffset.Now)
                return new BadRequest("Dogadjaj se još nije završio");

            if (activity.EndDate < DateTimeOffset.Now.AddDays(-7))
                return new BadRequest("Prošlo je nedelju dana od završetka Događaja!");

            var userId = _userAccessor.GetUserIdFromAccessToken();

            if (activity.User.Id != userId)
                return new BadRequest("Ne možete završiti tudji događaj");

            if (activity.HappeningMedias.Count > 0)
                return new BadRequest("Već ste završili događaj");

            foreach (var image in happeningUpdate.Images)
            {
                var photoResult = image != null ? await _photoAccessor.AddPhotoAsync(image) : null;
                if (photoResult != null)
                    activity.HappeningMedias.Add(new HappeningMedia() { PublicId = photoResult.PublicId, Url = photoResult.Url });
            }

            await _uow.CompleteAsync();

            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> ApproveHappeningCompletitionAsync(int id, bool approve)
        {
            var activity = await _uow.Activities.GetAsync(id);

            if (activity == null)
                return new NotFound("Aktivnost nije pronađena");

            if (activity.ActivityTypeId != ActivityTypeId.Happening)
                return new BadRequest("Aktivnost nije Događaj");

            if (activity.EndDate > DateTimeOffset.Now)
                return new BadRequest("Događaj se još nije završio");

            if (activity.HappeningMedias.Count == 0)
                return new BadRequest("Morate priložiti slike sa događaja");

            if (approve)
            {
                foreach (var media in activity.HappeningMedias)
                {
                    activity.ActivityMedias.Add(new ActivityMedia { PublicId = media.PublicId, Url = media.Url });
                }

                foreach (var attendance in activity.UserAttendances)
                {
                    if (attendance.Confirmed)
                    {
                        var userSkill = await _uow.Skills.GetHappeningSkillAsync(attendance.UserId);
                        var xpMultiplier = userSkill != null && userSkill.IsInSecondTree() ? await _uow.SkillXpBonuses.GetSkillMultiplierAsync(userSkill) : 1;

                        var xpIncrease = 250 * xpMultiplier;
                        attendance.User.CurrentXp += xpIncrease;
                    }
                    else
                        _uow.UserAttendaces.Remove(attendance);
                }

                var creatorSkill = await _uow.Skills.GetHappeningSkillAsync(activity.User.Id);
                var xpMultiplierForCreator = creatorSkill != null && creatorSkill.IsInSecondTree() ? await _uow.SkillXpBonuses.GetSkillMultiplierAsync(creatorSkill) : 1;

                var xpIncreaseForCreator = 250 * xpMultiplierForCreator * activity.UserAttendances.Count();
                activity.User.CurrentXp += xpIncreaseForCreator;
            }
            else
                activity.HappeningMedias
                    .ToList()
                    .ForEach(async m => await _photoAccessor.DeletePhotoAsync(m.PublicId));

            _uow.HappeningMedias.RemoveRange(activity.HappeningMedias);

            await _uow.CompleteAsync();

            await _emailManager.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true);

            return Unit.Default;
        }

        public async Task<Either<RestError, ApprovedActivityEnvelope>> GetApprovedActivitiesForUserAsync(UserQuery userQuery)
        {
            var activities = await _uow.Activities.GetActivitiesForUser(userQuery);

            if (activities == null)
                return new NotFound("Nema odobrenih aktivnosti za trenutno ulogovanog korisnika");

            return new ApprovedActivityEnvelope
            {
                Activities = _mapper.Map<IEnumerable<Activity>, IEnumerable<ApprovedActivityReturn>>(activities).ToList(),
                ActivityCount = await _uow.Activities.CountActivitiesFromUser(userQuery.UserId),
            };
        }
    }
}
