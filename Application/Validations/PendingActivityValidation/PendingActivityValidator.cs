using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Domain;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validations.PendingActivityValidation
{
    public class PendingActivityValidator : AbstractValidator<PendingActivity>
    {
        public PendingActivityValidator(IUserAccessor userAccessor)
        {
            var userId = userAccessor.GetUserIdFromAccessToken();

            RuleSet(nameof(UpdatePendingActivityValidator), () => Include(new UpdatePendingActivityValidator(userId)));
        }

        protected override bool PreValidate(ValidationContext<PendingActivity> context, ValidationResult result)
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
