using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Validations.ActivityValidation.Challenge;
using Application.Validations.ActivityValidation.Happening;
using Application.Validations.ActivityValidation.Puzzle;
using Domain;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validation.ActivityValidation
{
    public class ActivityValidator : AbstractValidator<Activity>
    {
        public ActivityValidator(IUserAccessor userAccessor)
        {
            var userId = userAccessor.GetUserIdFromAccessToken();

            RuleSet(nameof(AnswerToPuzzleValidator), () => Include(new AnswerToPuzzleValidator(userId)));

            RuleSet(nameof(CompleteHappeningValidator), () => Include(new CompleteHappeningValidator(userId)));
            RuleSet(nameof(ApproveHappeningCompletitionValidator), () => Include(new ApproveHappeningCompletitionValidator()));
            RuleSet(nameof(ConfirmAttendenceToHappeningValidator), () => Include(new ConfirmAttendenceToHappeningValidator(userId)));
            RuleSet(nameof(AttendToHappeningValidator), () => Include(new AttendToHappeningValidator(userId)));

            RuleSet(nameof(GetOwnerChallengeAnswersValidator), () => Include(new GetOwnerChallengeAnswersValidator(userId)));
            RuleSet(nameof(AnswerToChallengeValidator), () => Include(new AnswerToChallengeValidator(userId)));

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
