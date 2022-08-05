using Application.Validation.ActivityValidation;
using Domain;
using FluentValidation;

namespace Application.Validations.ActivityValidation.Happening
{
    public class AttendToHappeningValidator : AbstractValidator<Activity>
    {
        public AttendToHappeningValidator(int userId)
        {
            RuleSet(nameof(AttendToHappeningValidator), () =>
            {
                RuleFor(a => a.ActivityTypeId).IsHappening();

                RuleFor(a => a.EndDate).NotEnded();

                RuleFor(a => a.User.Id).NotOwner(userId);
            });
        }
    }
}
