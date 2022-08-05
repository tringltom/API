using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Validations.ActivityValidation.Challenge;
using Domain;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validations.ActivityValidation
{
    public class ChallengeAnswerValidator : AbstractValidator<UserChallengeAnswer>
    {
        public ChallengeAnswerValidator(IUserAccessor userAccessor)
        {
            var userId = userAccessor.GetUserIdFromAccessToken();

            RuleSet(nameof(ConfirmChallengeValidator), () => Include(new ConfirmChallengeValidator(userId)));
            RuleSet(nameof(ApproveChallengeAnswerValidator), () => Include(new ApproveChallengeAnswerValidator()));

        }

        protected override bool PreValidate(ValidationContext<UserChallengeAnswer> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                var error = new ValidationFailure
                {
                    CustomState = new NotFound("Odgovor nije pronađen")
                };

                result.Errors.Add(error);

                return false;
            }

            return true;
        }
    }
}
