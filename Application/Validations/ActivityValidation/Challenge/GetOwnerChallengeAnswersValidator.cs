using Application.Errors;
using Application.Validation.ActivityValidation;
using Domain;
using FluentValidation;

namespace Application.Validations.ActivityValidation.Challenge
{
    public class GetOwnerChallengeAnswersValidator : AbstractValidator<Activity>
    {
        public GetOwnerChallengeAnswersValidator(int userId)
        {
            RuleSet(nameof(GetOwnerChallengeAnswersValidator), () =>
            {
                RuleFor(a => a.ActivityTypeId).IsChallenge();

                RuleFor(a => a.EndDate).NotEnded();

                RuleFor(a => a.XpReward).Null()
                .WithState(e => new BadRequest("Izazov je rešen"));

                RuleFor(a => a.User.Id).IsOwner(userId);
            });
        }
    }
}
