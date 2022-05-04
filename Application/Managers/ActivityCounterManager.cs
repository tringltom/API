using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.ManagerInterfaces;
using Application.Models.Activity;
using DAL;
using Domain;

namespace Application.Managers
{
    public class ActivityCounterManager : IActivityCounterManager
    {
        private readonly IUnitOfWork _uow;

        public ActivityCounterManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ActivityCount>> GetActivityCountsAsync(User user)
        {
            var activityCountersForDelete = user.ActivityCreationCounters.Where(ac => ac.DateCreated.AddDays(7) < DateTimeOffset.Now).ToList();

            if (activityCountersForDelete.Count > 0)
            {
                _uow.ActivityCreationCounters.RemoveRange(activityCountersForDelete);
                await _uow.CompleteAsync();
            }

            var usedActivitiesCount = user.ActivityCreationCounters
                .GroupBy(acc => acc.ActivityTypeId)
                .Select(ac => new { Type = ac.Key, UsedCount = ac.Count() })
                .ToList();

            var skills = await _uow.Skills.GetSkillsAsync(user.Id);
            var skillActivities = await _uow.SkillActivities.GetAllAsync();

            var activitySkillBonuses = Enum.GetValues(typeof(ActivityTypeId)).OfType<ActivityTypeId>()
                .GroupJoin(skills,
                atEnum => atEnum,
                ac => ac.ActivityTypeId,
                (type, iskills) => new
                {
                    Type = type,
                    SkillActivityBonus = skillActivities
                    .SingleOrDefault(ab => ab.Level == (iskills.FirstOrDefault()?.Level > 3 ? 3 : iskills.FirstOrDefault() != null ? iskills.FirstOrDefault().Level : 0))
                }).ToList();

            return Enum.GetValues(typeof(ActivityTypeId)).OfType<ActivityTypeId>()
                .GroupJoin(usedActivitiesCount,
                    atEnum => atEnum,
                    ac => ac.Type,
                    (type, counts) => new
                    {
                        Type = type,
                        Used = counts.Select(used => used.UsedCount).FirstOrDefault()
                    })
                .Join(activitySkillBonuses,
                    inner => inner.Type,
                    sbs => sbs.Type,
                    (inter, sbs) => new ActivityCount
                    {
                        Type = inter.Type,
                        Available = sbs?.SkillActivityBonus?.Counter - inter.Used,
                        Max = sbs?.SkillActivityBonus?.Counter
                    })
                .ToList();
        }
    }
}
