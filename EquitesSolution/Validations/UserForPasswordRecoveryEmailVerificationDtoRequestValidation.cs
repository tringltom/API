using API.DTOs.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Validations
{
    public class UserForPasswordRecoveryEmailVerificationDtoRequestValidation : AbstractValidator<UserForPasswordRecoveryEmailVerificationDtoRequest>
    {
        public UserForPasswordRecoveryEmailVerificationDtoRequestValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token ne sme biti prazan");
            RuleFor(x => x.NewPassword).Password();
        }
    }
}
