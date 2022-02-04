using FluentValidation;
using Models.Activity;

namespace API.Validations
{
    public class FavoriteActivityBaseValidation : AbstractValidator<FavoriteActivityBase>
    {
        public FavoriteActivityBaseValidation()
        {
            RuleFor(f => f.UserId).GreaterThan(0).WithMessage("Nevalidan korisnik");
            RuleFor(f => f.ActivityId).GreaterThan(0).WithMessage("Nevalidna aktivnost");
        }
    }
}
