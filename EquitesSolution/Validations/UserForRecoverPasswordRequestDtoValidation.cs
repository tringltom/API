﻿using API.DTOs.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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