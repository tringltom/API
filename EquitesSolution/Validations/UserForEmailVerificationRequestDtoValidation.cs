﻿using API.DTOs.User;
using FluentValidation;

namespace API.Validations
{
    public class UserForEmailVerificationRequestDtoValidation : AbstractValidator<UserForEmailVerificationRequestDto>
    {
        public UserForEmailVerificationRequestDtoValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token ne sme biti prazan");
        }
    }
}
