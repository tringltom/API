using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;
using Application.ServiceInterfaces;
using Domain;

namespace Application.Services
{
    public class SkillService : ISkillService
    {
        public async Task<SkillData> GetSkillsDataAsync(int userId)
        {
            var testdataGd = new SkillLevel()
            {
                Level = 6,
                Type = ActivityTypeId.GoodDeed
            };

            var testdataj = new SkillLevel()
            {
                Level = 3,
                Type = ActivityTypeId.Joke
            };

            var testdatap = new SkillLevel()
            {
                Level = 0,
                Type = ActivityTypeId.Puzzle
            };

            var testdataq = new SkillLevel()
            {
                Level = 0,
                Type = ActivityTypeId.Quote
            };

            var testdatah = new SkillLevel()
            {
                Level = 0,
                Type = ActivityTypeId.Happening
            };

            var testdatac = new SkillLevel()
            {
                Level = 0,
                Type = ActivityTypeId.Challenge
            };


            var skillLevels = new List<SkillLevel>();
            skillLevels.Add(testdataGd);
            skillLevels.Add(testdataj);
            skillLevels.Add(testdatap);
            skillLevels.Add(testdataq);
            skillLevels.Add(testdatah);
            skillLevels.Add(testdatac);



            var testDAta = new SkillData()
            {
                CurrentLevel = 10,
                XpLevel = 15,
                SkillLevels = skillLevels
            };

            return await Task.FromResult(testDAta);
        }

        public async Task ResetSkillsDataAsync()
        {
            //reset skills
            await Task.Run(() => new SkillData());
        }

        public async Task UpdateSkillsDataAsync(SkillData skillData)
        {
            //update skills
            await Task.Run(() => new SkillData());
        }
    }
}
