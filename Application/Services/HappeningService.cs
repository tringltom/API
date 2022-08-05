using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using Application.Validations.ActivityValidation.Happening;
using AutoMapper;
using DAL;
using DAL.Query;
using Domain;
using FluentValidation;
using LanguageExt;

namespace Application.Services
{
    public class HappeningService : IHappeningService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IUserAccessor _userAccessor;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IEmailManager _emailManager;
        private readonly IValidator<Activity> _activityValidator;

        public HappeningService(IMapper mapper,
            IUnitOfWork uow,
            IUserAccessor userAccessor,
            IPhotoAccessor photoAccessor,
            IEmailManager emailManager,
            IValidator<Activity> activityValidator)
        {
            _mapper = mapper;
            _uow = uow;
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
            _emailManager = emailManager;
            _activityValidator = activityValidator;
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

        public async Task<Either<RestError, Unit>> AttendToHappeningAsync(int id, bool attend)
        {
            var activity = await _uow.Activities.GetAsync(id);

            var validation = _activityValidator
                .Validate(activity, o => o.IncludeRuleSets(nameof(AttendToHappeningValidator)));

            if (!validation.IsValid)
                return (RestError)validation.Errors.Single().CustomState;

            var userId = _userAccessor.GetUserIdFromAccessToken();

            var userAttendence = activity.UserAttendances.SingleOrDefault(ua => ua.UserId == userId);

            if (attend && userAttendence == null)
                _uow.UserAttendaces.Add(new UserAttendance { UserId = userId, ActivityId = id, Confirmed = false });
            if (!attend && userAttendence != null)
                _uow.UserAttendaces.Remove(userAttendence);

            await _uow.CompleteAsync();

            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> ConfirmAttendenceToHappeningAsync(int id)
        {
            var activity = await _uow.Activities.GetAsync(id);

            var validation = _activityValidator
                .Validate(activity, o => o.IncludeRuleSets(nameof(ConfirmAttendenceToHappeningValidator)));

            if (!validation.IsValid)
                return (RestError)validation.Errors.Single().CustomState;

            var userId = _userAccessor.GetUserIdFromAccessToken();

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

            var validation = _activityValidator
                .Validate(activity, o => o.IncludeRuleSets(nameof(CompleteHappeningValidator)));

            if (!validation.IsValid)
                return (RestError)validation.Errors.Single().CustomState;

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

            var validation = _activityValidator
                .Validate(activity, o => o.IncludeRuleSets(nameof(ApproveHappeningCompletitionValidator)));

            if (!validation.IsValid)
                return (RestError)validation.Errors.Single().CustomState;

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
    }
}
