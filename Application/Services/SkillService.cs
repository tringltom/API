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

        public async Task<SkillData> GetSkillsDataAsync(int userId)
        {
            var skills = await _uow.Skills.GetSkills(userId);
            var user = await _uow.Users.GetAsync(userId);

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
                XpLevel = await _uow.XpLevels.GetPotentialLevel(user.CurrentXp),
            };
        }

        public async Task<UserBaseResponse> ResetSkillsDataAsync()
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var skills = await _uow.Skills.GetSkills(userId);

            if (skills == null || skills.Sum(s => s?.Level) == 0)
                throw new NotFound("Nemate odabrane veštine");

            var user = await _uow.Users.GetAsync(userId);

            if (user.SkillSpecial != null)
                user.SkillSpecial = null;

            foreach (var skill in skills)
            {
                skill.Level = 0;
            }

            user.XpLevelId = 1;

            await _uow.CompleteAsync();

            var userResponse = _mapper.Map<UserBaseResponse>(user);
            userResponse.ActivityCounts = await _activityCounterManager.GetActivityCounts(user);

            return userResponse;
        }

        public async Task<UserBaseResponse> UpdateSkillsDataAsync(SkillData skillData)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);
            var potentialLevel = await _uow.XpLevels.GetPotentialLevel(user.CurrentXp);

            if (skillData.XpLevel != potentialLevel)
                throw new BadRequest("Niste odgovarajući nivo!");

            var skills = await _uow.Skills.GetSkills(userId);

            foreach (var skillLevel in skillData.SkillLevels)
            {
                if (skills.Any(s => s.ActivityTypeId == skillLevel.Type))
                    skills.Where(s => s.ActivityTypeId == skillLevel.Type).SingleOrDefault().Level = skillLevel.Level;
                else
                    _uow.Skills.Add(new Skill { ActivityTypeId = skillLevel.Type, Level = skillLevel.Level, User = user });
            }

            user.XpLevelId = skills.Sum(s => s.Level) + 1;

            var specialAbilities = skills.Where(s => s.IsInThirdTree());
            user.SkillSpecial = await _uow.SkillSpecials.GetSkillSpecial(specialAbilities.ElementAtOrDefault(0)?.ActivityTypeId, specialAbilities.ElementAtOrDefault(1)?.ActivityTypeId);

            await _uow.CompleteAsync();

            var userResponse = _mapper.Map<UserBaseResponse>(user);
            userResponse.ActivityCounts = await _activityCounterManager.GetActivityCounts(user);

            return userResponse;
        }
    }
}
