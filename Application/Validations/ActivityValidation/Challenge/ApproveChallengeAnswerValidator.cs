using Application.Errors;
using Domain;
using FluentValidation;

namespace Application.Validations.ActivityValidation.Challenge
{
    public class ApproveChallengeAnswerValidator : AbstractValidator<UserChallengeAnswer>
    {
        public ApproveChallengeAnswerValidator()
        {
            RuleSet(nameof(ApproveChallengeAnswerValidator), () =>
            {
                RuleFor(a => a.Confirmed).Equal(true)
                .WithState(e => new BadRequest("Ovaj odgovor nije odabran"));

                RuleFor(a => a.Activity.XpReward).Null()
                .WithState(e => new BadRequest("Izazov je rešen"));
            });
        }
    }
}
