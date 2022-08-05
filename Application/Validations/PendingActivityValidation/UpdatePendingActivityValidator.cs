using Application.Errors;
using Domain;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validations.PendingActivityValidation
{
    public class UpdatePendingActivityValidator : AbstractValidator<PendingActivity>
    {
        public UpdatePendingActivityValidator(int userId)
        {
            RuleSet(nameof(UpdatePendingActivityValidator), () =>
            {
                RuleFor(a => a.User.Id).Equal(userId)
                    .WithState(e => new BadRequest("Niste kreirali ovu aktivnost!"));

                RuleFor(a => a.ActivityTypeId).Custom((x, context) =>
                {
                    if (!context.RootContextData.TryGetValue(nameof(ActivityTypeId), out var type))
                        return;

                    if (x != (ActivityTypeId)type)
                    {
                        var error = new ValidationFailure
                        {
                            CustomState = new BadRequest("Ne možete izmeniti tip aktivnosti!")
                        };

                        context.AddFailure(error);
                    }
                });
            });
        }
    }
}
