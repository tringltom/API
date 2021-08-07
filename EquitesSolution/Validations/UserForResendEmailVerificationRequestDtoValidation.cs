using API.DTOs.User;
using FluentValidation;

namespace API.Validations
{
    public class UserForResendEmailVerificationRequestDtoValidation : AbstractValidator<UserForResendEmailVerificationRequestDto>
    {
        public UserForResendEmailVerificationRequestDtoValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
        }
    }
}
