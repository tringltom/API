using System.Linq;
using Application.Errors;
using Application.Models.Activity;
using Domain;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validations.ActivityValidation.Puzzle
{
    internal class AnswerToPuzzleValidator : AbstractValidator<Activity>
    {
        public AnswerToPuzzleValidator(int userId)
        {
            RuleSet(nameof(AnswerToPuzzleValidator), () =>
            {
                RuleFor(a => a.ActivityTypeId).Equal(ActivityTypeId.Puzzle)
                .WithState(e => new BadRequest("Aktivnost nije zagonetka"));

                RuleFor(a => a.Answer.Trim().ToLower()).Custom((x, context) =>
                {
                    if (!context.RootContextData.TryGetValue(nameof(PuzzleAnswer), out var answer))
                        return;

                    if (x != answer.ToString().Trim().ToLower())
                    {
                        var error = new ValidationFailure
                        {
                            CustomState = new BadRequest("Netačan odgovor")
                        };

                        context.AddFailure(error);
                    }
                });

                RuleFor(a => a.User.Id).NotEqual(userId)
                .WithState(e => new BadRequest("Ne možete odgovarati na svoje zagonetke"));

                RuleFor(a => a.UserPuzzleAnswers.SingleOrDefault(ua => ua.UserId == userId)).Null()
                .WithState(e => new BadRequest("Već ste dali tačan odgovor"));
            });
        }
    }
}
