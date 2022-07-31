using Application.Models.Activity;
using FluentValidation;

namespace API.Validations
{
    public class HappeningUpdateValidator : AbstractValidator<HappeningUpdate>
    {
        public HappeningUpdateValidator()
        {
            RuleFor(x => x.Images).NotEmpty().WithMessage("Zavrsen Dogadjaj mora imati makar jednu sliku");
        }
    }
}
