﻿using FluentValidation;
using Models.User;

namespace API.Validations
{
    public class UserLoginValidation : AbstractValidator<UserLogin>
    {
        public UserLoginValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Configure(rule => rule.MessageBuilder = _ => "Neispravna email adresa");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Šifra ne sme biti prazna");
        }
    }
}