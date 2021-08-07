using API.DTOs.User;
using FluentValidation;

namespace API.Validations
{
    public class UserForRecoverPasswordRequestDtoValidation : AbstractValidator<UserForRecoverPasswordRequestDto>
    {
        public UserForRecoverPasswordRequestDtoValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
        }
    }
}
