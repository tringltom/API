using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.ManagerInterfaces;
using Application.Models;
using Application.Models.User;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;
using LanguageExt;

namespace Application.Services
{
    public class SkillService : ISkillService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IActivityCounterManager _activityCounterManager;

        public SkillService(IUserAccessor userAccessor, IUnitOfWork uow, IMapper mapper, IActivityCounterManager activityCounterManager)
        {
            _userAccessor = userAccessor;
            _uow = uow;
            _mapper = mapper;
            _activityCounterManager = activityCounterManager;
        }

        public async Task<Either<RestError, SkillData>> GetSkillsDataAsync(int userId)
        {
            var skills = await _uow.Skills.GetSkillsAsync(userId);
            var user = await _uow.Users.GetAsync(userId);

            if (user == null)
                return new NotFound("Nepostojeci korisnik");

            return new SkillData
            {
                CurrentLevel = user.XpLevelId,
                SkillLevels = Enum.GetValues(typeof(ActivityTypeId)).OfType<ActivityTypeId>()
                    .GroupJoin(skills,
                    atEnum => atEnum,
                    sl => sl.ActivityTypeId,
                    (type, levels) => new SkillLevel
                    {
                        Type = type,
                        Level = levels.Select(l => l.Level).FirstOrDefault()
                    }).ToList(),
                XpLevel = await _uow.XpLevels.GetPotentialLevelAsync(user.CurrentXp),
            };
        }

        public async Task<Either<RestError, UserBaseResponse>> UpdateSkillsDataAsync(SkillData skillData)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);
            var potentialLevel = await _uow.XpLevels.GetPotentialLevelAsync(user.CurrentXp);

            if (skillData.XpLevel > potentialLevel)
                return new BadRequest("Niste odgovarajući nivo!");

            var skills = await _uow.Skills.GetSkillsAsync(userId);

            foreach (var skillLevel in skillData.SkillLevels)
            {
                if (skills.Any(s => s.ActivityTypeId == skillLevel.Type))
                    skills.Where(s => s.ActivityTypeId == skillLevel.Type).SingleOrDefault().Level = skillLevel.Level;
                else
                    _uow.Skills.Add(new Skill { ActivityTypeId = skillLevel.Type, Level = skillLevel.Level, User = user });
            }

            user.XpLevelId = skills.Sum(s => s.Level) + 1;

            var specialAbilities = skills.Where(s => s.IsInThirdTree());
            var skillSpecial = await _uow.SkillSpecials.GetSkillSpecialAsync(specialAbilities.ElementAtOrDefault(0)?.ActivityTypeId, specialAbilities.ElementAtOrDefault(1)?.ActivityTypeId);
            user.SkillSpecial = skillSpecial;

            await _uow.CompleteAsync();

            var userResponse = _mapper.Map<UserBaseResponse>(user);
            userResponse.ActivityCounts = await _activityCounterManager.GetActivityCountsAsync(user);

            return userResponse;
        }
    }
}
