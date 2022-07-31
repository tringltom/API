using Application.Models.Activity;
using FluentValidation;

namespace API.Validations
{
    public class PuzzleAnswerValidation : AbstractValidator<PuzzleAnswer>
    {
        public PuzzleAnswerValidation()
        {
            RuleFor(x => x.Answer)
                .NotEmpty().WithMessage("Odgovor ne sme biti prazan")
                .MaximumLength(100).WithMessage("Odgovor ne sme imati više od 100 karaktera");
        }
    }
}
