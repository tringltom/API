using Application.Models.User;
using FluentValidation;

namespace API.Validations
{
    public class UserImageValidation : AbstractValidator<UserImageUpdate>
    {
        public UserImageValidation()
        {
            RuleFor(x => x.Image).NotEmpty().WithMessage("Slika je obavezna");
        }
    }
}
