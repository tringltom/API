using API.DTOs.User;
using FluentValidation;

namespace API.Validations
{
    public class UserForRegistrationRequestDtoValidation : AbstractValidator<UserForRegistrationRequestDto>
    {
        public UserForRegistrationRequestDtoValidation()
        {
            RuleFor(x => x.UserName).MinimumLength(4).MaximumLength(30).Configure(rule => rule.MessageBuilder = _ => "Korisničko ime mora biti između 4 i 30 karaktera");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
            RuleFor(x => x.Password).Password();
        }
    }
}
