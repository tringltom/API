using FluentValidation;
using Models.Activity;

namespace API.Validations
{
    public class FavoriteActivityBaseValidation : AbstractValidator<FavoriteActivityBase>
    {
        public FavoriteActivityBaseValidation()
        {
            RuleFor(f => f.ActivityId).GreaterThan(0).WithMessage("Nevalidna aktivnost");
        }
    }
}
