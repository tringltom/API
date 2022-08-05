using Application.Errors;
using Application.Validation.ActivityValidation;
using Domain;
using FluentValidation;

namespace Application.Validations.ActivityValidation.Challenge
{
    public class ConfirmChallengeValidator : AbstractValidator<UserChallengeAnswer>
    {
        public ConfirmChallengeValidator(int userId)
        {
            RuleSet(nameof(ConfirmChallengeValidator), () =>
            {
                RuleFor(a => a.Confirmed).Equal(false)
                .WithState(e => new BadRequest("Ovaj odgovor ste već odabrali"));

                RuleFor(a => a.Activity.User.Id).IsOwner(userId);
            });
        }
    }
}
