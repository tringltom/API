using System;
using Application.Errors;
using Application.Validation.ActivityValidation;
using Domain;
using FluentValidation;

namespace Application.Validations.ActivityValidation.Happening
{
    internal class CompleteHappeningValidator : AbstractValidator<Activity>
    {
        public CompleteHappeningValidator(int userId)
        {
            RuleSet(nameof(CompleteHappeningValidator), () =>
            {
                RuleFor(a => a.ActivityTypeId).IsHappening();

                RuleFor(a => a.EndDate).IsEnded();

                RuleFor(a => a.EndDate).GreaterThan(DateTimeOffset.Now.AddDays(-7))
                .WithState(e => new BadRequest("Prošlo je nedelju dana od završetka Događaja"));

                RuleFor(a => a.User.Id).NotOwner(userId);

                RuleFor(a => a.HappeningMedias).Empty()
                .WithState(e => new BadRequest("Već ste završili događaj"));
            });
        }
    }
}
