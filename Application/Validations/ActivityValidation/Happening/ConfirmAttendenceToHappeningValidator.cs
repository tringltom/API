using System;
using Application.Errors;
using Application.Validation.ActivityValidation;
using Domain;
using FluentValidation;

namespace Application.Validations.ActivityValidation.Happening
{
    public class ConfirmAttendenceToHappeningValidator : AbstractValidator<Activity>
    {
        public ConfirmAttendenceToHappeningValidator(int userId)
        {
            RuleSet(nameof(ConfirmAttendenceToHappeningValidator), () =>
            {
                RuleFor(a => a.ActivityTypeId).IsHappening();

                RuleFor(a => a.EndDate).NotEnded();

                RuleFor(a => a.StartDate).LessThanOrEqualTo(DateTimeOffset.Now)
                .WithState(e => new BadRequest("Događaj još nije počeo"));

                RuleFor(a => a.User.Id).NotOwner(userId);
            });
        }
    }
}
