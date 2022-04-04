﻿using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface ISkillXpBonusRepository : IBaseRepository<SkillXpBonus>
    {
        Task<int> GetSkillMultiplier(Skill skill);
    }
}
