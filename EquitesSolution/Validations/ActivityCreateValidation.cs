using Domain;
using FluentValidation;
using Models.Activity;

namespace API.Validations
{
    public class ActivityCreateValidation : AbstractValidator<ActivityCreate>
    {
        public ActivityCreateValidation()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Naziv ne sme biti prazan")
                .MaximumLength(50).WithMessage("Naziv ne sme imati više od 50 karaktera");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Opis ne sme imati više od 250 karaktera")
                .NotEmpty().WithMessage("Opis ne sme biti prazan ako slika nije priložena")
                    .When(x => x.Images == null, ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.Answer)
                .NotEmpty().WithMessage("Odgovor ne sme biti prazan")
                .MaximumLength(100).WithMessage("Odgovor ne sme imati više od 100 karaktera")
                .When(x => x.Type == ActivityTypeId.Puzzle);

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Mora postojati početni datum aktivnosti ukoliko je kraj iste određen")
                .LessThan(x => x.EndDate).WithMessage("Datum završetka aktivnosti ne sme biti pre početnog datuma iste")
                .When(x => x.EndDate != null);

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Događaj mora imati lokaciju")
                .When(x => x.Type == ActivityTypeId.Happening);

            RuleFor(x => x.Images)
                .NotEmpty().WithMessage("Dobro Delo mora imati makar jednu sliku")
                .When(x => x.Type == ActivityTypeId.GoodDeed);
        }
    }
}
