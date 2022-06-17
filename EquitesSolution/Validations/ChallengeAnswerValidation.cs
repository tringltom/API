using Application.Models.Activity;
using FluentValidation;

namespace API.Validations
{
    public class ChallengeAnswerValidation : AbstractValidator<ChallengeAnswer>
    {
        public ChallengeAnswerValidation()
        {
            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Opis ne sme imati više od 250 karaktera")
                .NotEmpty().WithMessage("Opis ne sme biti prazan ako slika nije priložena")
                    .When(x => x.Images == null, ApplyConditionTo.CurrentValidator);
        }
    }
}
