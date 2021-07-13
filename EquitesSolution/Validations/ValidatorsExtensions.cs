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
                .MinimumLength(6).WithMessage("Sifra mora imati barem 6 karaktera.")
                .Matches("[A-Z]").WithMessage("Sifra mora sadrzati barem jedno veliko slovo.")
                .Matches("[a-z]").WithMessage("Sifra mora sadrzati barem jedno malo slovo.")
                .Matches("[0-9]").WithMessage("Sifra mora sadrzati barem jedan broj.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Sifra mora sadrzati barem jedan alfanumericki karakter.");

            return options;
        }
    }
}
