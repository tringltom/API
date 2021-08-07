using API.DTOs.User;
using FluentValidation;

namespace API.Validations
{
    public class UserForPasswordChangeRequestDtoValidation : AbstractValidator<UserForPasswordChangeRequestDto>
    {

        public UserForPasswordChangeRequestDtoValidation()
        {
            RuleFor(x => x.NewPassword).Password();
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Šifra ne sme biti prazna");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
        }
    }
}
