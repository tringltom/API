using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Validations
{
    public static class ValidatorsExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty()
                .MinimumLength(6).WithMessage("Šifra mora imati barem 6 karaktera.")
                .Matches("[A-Z]").WithMessage("Šifra mora sadržati barem jedno veliko slovo.")
                .Matches("[a-z]").WithMessage("Šifra mora sadržati barem jedno malo slovo.")
                .Matches("[0-9]").WithMessage("Šifra mora sadržati barem jedan broj.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Šifra mora sadržati barem jedan alfanumerički karakter.");

            return options;
        }
    }
}
