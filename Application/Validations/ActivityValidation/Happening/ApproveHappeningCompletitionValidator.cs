using Application.Errors;
using Application.Validation.ActivityValidation;
using Domain;
using FluentValidation;

namespace Application.Validations.ActivityValidation.Happening
{
    public class ApproveHappeningCompletitionValidator : AbstractValidator<Activity>
    {
        public ApproveHappeningCompletitionValidator()
        {
            RuleSet(nameof(ApproveHappeningCompletitionValidator), () =>
            {
                RuleFor(a => a.ActivityTypeId).IsHappening();

                RuleFor(a => a.EndDate).IsEnded();

                RuleFor(a => a.HappeningMedias.Count).GreaterThan(0)
                .WithState(e => new BadRequest("Morate priložiti slike sa događaja"));
            });
        }
    }
}
