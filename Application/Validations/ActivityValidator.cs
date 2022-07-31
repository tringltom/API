using System;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Domain;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validations
{
    public class ActivityValidator : AbstractValidator<Activity>
    {
        public ActivityValidator(IUserAccessor userAccessor)
        {
            var userId = userAccessor.GetUserIdFromAccessToken();

            RuleSet(Rules.CompleteHappening, () =>
                {
                    RuleFor(a => a.ActivityTypeId).IsHappening()
                    .WithState(x => new BadRequest("Aktivnost nije Događaj"))
                        .DependentRules(() =>
                        {
                            RuleFor(a => a.EndDate).LessThanOrEqualTo(DateTimeOffset.Now)
                            .WithState(x => new BadRequest("Događaj se još nije završio"))
                            .DependentRules(() =>
                            {
                                RuleFor(a => a.EndDate).GreaterThan(DateTimeOffset.Now.AddDays(-7))
                                .WithState(x => new BadRequest("Prošlo je nedelju dana od završetka Događaja"))
                                .DependentRules(() =>
                                {
                                    RuleFor(a => a.User.Id).Equal(userId)
                                    .WithState(x => new BadRequest("Ne možete završiti tuđi događaj"))
                                    .DependentRules(() =>
                                    {
                                        RuleFor(a => a.HappeningMedias).Empty()
                                        .WithState(x => new BadRequest("Već ste završili događaj"));
                                    });
                                });
                            });
                        });
                });
        }

        protected override bool PreValidate(ValidationContext<Activity> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                var error = new ValidationFailure
                {
                    CustomState = new NotFound("Aktivnost nije pronađena")
                };

                result.Errors.Add(error);

                return false;
            }

            return true;
        }
    }
}
