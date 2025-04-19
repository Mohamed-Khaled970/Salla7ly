using FluentValidation;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Validators
{
    public class SignInCommandValidator : AbstractValidator<SignInCommand>
    {
        public SignInCommandValidator()
        {

            RuleFor(x => x.Password)
                    .NotNull()
                    .NotEmpty();

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress().WithMessage("Invalid email address format.");


        }
    }
}
