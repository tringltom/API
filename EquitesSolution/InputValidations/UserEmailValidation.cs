using Application.Models.User;
using FluentValidation;

namespace API.Validations
{
    public class UserEmailValidation : AbstractValidator<UserEmail>
    {
        public UserEmailValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
        }
    }
}
