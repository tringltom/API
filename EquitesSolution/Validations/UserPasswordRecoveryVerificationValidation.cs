using Models.User;
using FluentValidation;

namespace API.Validations;

public class UserPasswordRecoveryVerificationValidation : AbstractValidator<UserPasswordRecoveryVerification>
{
    public UserPasswordRecoveryVerificationValidation()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token ne sme biti prazan");
        RuleFor(x => x.NewPassword).Password();
    }
}

