using Domain;
using FluentValidation;

namespace Application.Validations
{
    public static class ActivityValidatorsExtensions
    {
        public static IRuleBuilderOptions<T, ActivityTypeId> IsHappening<T>(this IRuleBuilder<T, ActivityTypeId> ruleBuilder)
        {
            return ruleBuilder.Equal(ActivityTypeId.Happening);
        }
    }
}
