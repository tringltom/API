using System;
using Application.Errors;
using Domain;
using FluentValidation;

namespace Application.Validation.ActivityValidation
{
    public static class ActivityValidatorsExtensions
    {
        public static IRuleBuilderOptions<T, ActivityTypeId> IsHappening<T>(this IRuleBuilder<T, ActivityTypeId> ruleBuilder)
        {
            return ruleBuilder.Equal(ActivityTypeId.Happening)
                .WithState(x => new BadRequest("Aktivnost nije Događaj"));
        }

        public static IRuleBuilderOptions<T, DateTimeOffset?> IsEnded<T>(this IRuleBuilder<T, DateTimeOffset?> ruleBuilder)
        {
            return ruleBuilder.LessThanOrEqualTo(DateTimeOffset.Now)
                .WithState(x => new BadRequest("Događaj se još nije završio"));
        }

        public static IRuleBuilderOptions<T, DateTimeOffset?> NotEnded<T>(this IRuleBuilder<T, DateTimeOffset?> ruleBuilder)
        {
            return ruleBuilder.LessThanOrEqualTo(DateTimeOffset.Now)
                .WithState(x => new BadRequest("Događaj se završio"));
        }

        public static IRuleBuilderOptions<T, int> NotOwner<T>(this IRuleBuilder<T, int> ruleBuilder, int userId)
        {
            return ruleBuilder.NotEqual(userId)
                .WithState(x => new BadRequest("Vi ste kreirali ovu aktivnost"));
        }

        public static IRuleBuilderOptions<T, int> IsOwner<T>(this IRuleBuilder<T, int> ruleBuilder, int userId)
        {
            return ruleBuilder.Equal(userId)
                .WithState(x => new BadRequest("Niste kreirali ovu aktivnost"));
        }

        public static IRuleBuilderOptions<T, ActivityTypeId> IsChallenge<T>(this IRuleBuilder<T, ActivityTypeId> ruleBuilder)
        {
            return ruleBuilder.Equal(ActivityTypeId.Challenge)
                .WithState(x => new BadRequest("Aktivnost nije Izazov"));
        }
    }
}
