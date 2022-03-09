using Application.Models.User;
using FluentValidation;

namespace API.Validations
{
    public class UserAboutValidation : AbstractValidator<UserAbout>
    {
        public UserAboutValidation()
        {
            RuleFor(x => x.About).MaximumLength(2000).WithMessage("Za opis je dozvoljeno maksimalno 2000 karaktera");
        }
    }
}
