using System.Collections.Generic;

namespace Application.Models
{
    public class SkillData
    {
        public int CurrentLevel { get; set; }
        public int XpLevel { get; set; }

        public ICollection<SkillLevel> SkillLevels { get; set; }
    }
}
