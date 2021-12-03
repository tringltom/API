using Models.User;

namespace API.Validations;

public class UserPasswordChangeValidation : AbstractValidator<UserPasswordChange>
{

    public UserPasswordChangeValidation()
    {
        RuleFor(x => x.NewPassword).Password();
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Šifra ne sme biti prazna");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
    }
}

