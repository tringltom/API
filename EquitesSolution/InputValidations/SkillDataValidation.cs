using System.Linq;
using Application.Models;
using FluentValidation;

namespace API.Validations
{
    public class SkillDataValidation : AbstractValidator<SkillData>
    {
        public SkillDataValidation()
        {
            RuleFor(s => s.CurrentLevel).GreaterThan(0).WithMessage("Nevalidan trenutni nivo");
            RuleFor(s => s.XpLevel)
                .GreaterThan(0).WithMessage("Nevalidan potencijalni nivo")
                .GreaterThanOrEqualTo(s => s.CurrentLevel).WithMessage("Potencijalni nivo ne može biti manji od trenutnog")
                .GreaterThan(s => s.SkillLevels.Sum(sl => sl.Level)).WithMessage("Potencijalni nivo ne može biti manji ili jendak broju ukupno izabranih veština");

            RuleForEach(sd => sd.SkillLevels).ChildRules(sl =>
            {
                sl.RuleFor(x => x.Level).InclusiveBetween(0, 7).WithMessage("Nevalidan nivo veštine");
            });
        }
    }
}
