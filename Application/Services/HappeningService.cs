using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class HappeningService : IHappeningService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IUserAccessor _userAccessor;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IEmailManager _emailManager;

        public HappeningService(IMapper mapper, IUnitOfWork uow, IUserAccessor userAccessor, IPhotoAccessor photoAccessor, IEmailManager emailManager)
        {
            _mapper = mapper;
            _uow = uow;
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
            _emailManager = emailManager;
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
                return new BadRequest("Događaj se još nije završio");

            if (activity.EndDate < DateTimeOffset.Now.AddDays(-7))
                return new BadRequest("Prošlo je nedelju dana od završetka Događaja!");

            var userId = _userAccessor.GetUserIdFromAccessToken();

            if (activity.User.Id != userId)
                return new BadRequest("Ne možete završiti tuđi događaj");

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
    }
}
