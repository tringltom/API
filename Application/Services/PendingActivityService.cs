using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using AutoMapper;
using DAL;
using Domain;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class PendingActivityService
    {
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly IEmailManager _emailManager;
        private readonly IUnitOfWork _uow;

        public PendingActivityService(IPhotoAccessor photoAccessor, IUserAccessor userAccessor, IMapper mapper, IEmailManager emailManager, IUnitOfWork uow)
        {
            _photoAccessor = photoAccessor;
            _userAccessor = userAccessor;
            _mapper = mapper;
            _emailManager = emailManager;
            _uow = uow;
        }

        public async Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset)
        {
            var pendingActivities = await _uow.PendingActivities.GetLatestPendingActivitiesAsync(limit, offset);

            return new PendingActivityEnvelope
            {
                Activities = _mapper.Map<IEnumerable<PendingActivity>, IEnumerable<PendingActivityReturn>>(pendingActivities).ToList(),
                ActivityCount = await _uow.PendingActivities.CountAsync()
            };
        }

        public async Task<PendingActivityForUserEnvelope> GetOwnerPendingActivitiesAsync(int? limit, int? offset)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var pendingActivities = await _uow.PendingActivities.GetLatestPendingActivitiesAsync(userId, limit, offset);

            return new PendingActivityForUserEnvelope
            {
                Activities = _mapper.Map<IEnumerable<PendingActivity>, IEnumerable<PendingActivityForUserReturn>>(pendingActivities).ToList(),
                ActivityCount = await _uow.PendingActivities.CountPendingActivitiesAsync(userId)
            };
        }

        public async Task<Either<RestError, PendingActivityReturn>> GetOwnerPendingActivityAsync(int id)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var pendingActivity = await _uow.PendingActivities.GetAsync(id);

            if (pendingActivity.User.Id != userId)
                return new BadRequest("Niste kreirali ovu aktivnost!");

            return _mapper.Map<PendingActivityReturn>(pendingActivity);
        }

        public async Task<Either<RestError, PendingActivityReturn>> UpdatePendingActivityAsync(int id, ActivityCreate updatedActivityCreate)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var pendingActivity = await _uow.PendingActivities.GetAsync(id);

            if (pendingActivity == null)
                return new NotFound("Aktivnost nije pronadjena");

            if (pendingActivity.User.Id != userId)
                return new BadRequest("Niste kreirali ovu aktivnost!");

            if (pendingActivity.ActivityTypeId != updatedActivityCreate.Type)
                return new BadRequest("Ne možete izmeniti tip aktivnosti!");

            pendingActivity.Title = updatedActivityCreate.Title;
            pendingActivity.Description = updatedActivityCreate.Description;
            pendingActivity.Answer = updatedActivityCreate.Answer;
            pendingActivity.Latitude = updatedActivityCreate.Latitude;
            pendingActivity.Longitude = updatedActivityCreate.Longitude;
            pendingActivity.StartDate = updatedActivityCreate.StartDate;
            pendingActivity.EndDate = updatedActivityCreate.EndDate;
            pendingActivity.Location = updatedActivityCreate.Location;

            pendingActivity.PendingActivityMedias = null;

            foreach (var image in updatedActivityCreate?.Images ?? new IFormFile[0])
            {
                var photoResult = image != null ? await _photoAccessor.AddPhotoAsync(image) : null;
                if (photoResult != null)
                    pendingActivity.PendingActivityMedias.Add(new PendingActivityMedia() { PublicId = photoResult.PublicId, Url = photoResult.Url });
            }

            await _uow.CompleteAsync();

            return _mapper.Map<PendingActivityReturn>(pendingActivity);
        }

        public async Task<Either<RestError, Unit>> DisapprovePendingActivityAsync(int id)
        {
            var pendingActivity = await _uow.PendingActivities.GetAsync(id);

            if (pendingActivity == null)
                return new NotFound("Aktivnost nije pronadjena");

            _uow.PendingActivities.Remove(pendingActivity);
            await _uow.CompleteAsync();

            pendingActivity.PendingActivityMedias
                .ToList()
                .ForEach(async m => await _photoAccessor.DeletePhotoAsync(m.PublicId));

            await _emailManager.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, false);

            return Unit.Default;
        }
        public async Task<Either<RestError, PendingActivityReturn>> CreatePendingActivityAsync(ActivityCreate activityCreate)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var activity = _mapper.Map<PendingActivity>(activityCreate);
            var skill = await _uow.Skills.GetSkillAsync(userId, activityCreate.Type);
            var skillActivities = await _uow.SkillActivities.GetAllAsync();
            var maxActivityCounter = skillActivities.FirstOrDefault(sa => sa.Level == (skill?.Level > 3 ? 3 : skill?.Level != null ? skill.Level : 0))?.Counter;
            var user = await _uow.Users.GetAsync(userId);

            if (user.ActivityCreationCounters
                .Where(ac => ac.ActivityTypeId == activityCreate.Type && ac.DateCreated.AddDays(7) >= DateTimeOffset.Now)
                .Count() >= maxActivityCounter)
            {
                return new BadRequest("Ne možete još uvek da kreirate aktivnost");
            }

            activity.User = user;

            foreach (var image in activityCreate?.Images ?? new IFormFile[0])
            {
                var photoResult = image != null ? await _photoAccessor.AddPhotoAsync(image) : null;
                if (photoResult != null)
                    activity.PendingActivityMedias.Add(new PendingActivityMedia() { PublicId = photoResult.PublicId, Url = photoResult.Url });
            }

            _uow.PendingActivities.Add(activity);

            var activityCreationCounter = new ActivityCreationCounter
            {
                User = activity.User,
                ActivityTypeId = activity.ActivityTypeId,
                DateCreated = DateTimeOffset.Now
            };

            _uow.ActivityCreationCounters.Add(activityCreationCounter);

            await _uow.CompleteAsync();

            return _mapper.Map<PendingActivityReturn>(activity);
        }
    }
}
