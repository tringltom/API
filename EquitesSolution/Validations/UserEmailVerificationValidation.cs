using FluentValidation;
using Models.User;

namespace API.Validations;

public class UserEmailVerificationValidation : AbstractValidator<UserEmailVerification>
{
    public UserEmailVerificationValidation()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token ne sme biti prazan");
    }
}

