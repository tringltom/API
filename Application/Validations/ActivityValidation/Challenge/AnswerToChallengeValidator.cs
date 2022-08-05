using Application.Validation.ActivityValidation;
using Domain;
using FluentValidation;

namespace Application.Validations.ActivityValidation.Challenge
{
    public class AnswerToChallengeValidator : AbstractValidator<Activity>
    {
        public AnswerToChallengeValidator(int userId)
        {
            RuleSet(nameof(AnswerToChallengeValidator), () =>
            {
                RuleFor(a => a.ActivityTypeId).IsChallenge();

                RuleFor(a => a.User.Id).NotOwner(userId);
            });
        }
    }
}
