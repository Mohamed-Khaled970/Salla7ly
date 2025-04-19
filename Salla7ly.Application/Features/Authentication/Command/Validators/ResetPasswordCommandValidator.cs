using FluentValidation;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Validators
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {

            RuleFor(x => x.Password)
                    .NotNull()
                    .NotEmpty()
                    .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")
                    .WithMessage("Password must be at least 8 characters long and include at least one uppercase letter, " +
                    "one lowercase letter, one digit, and one special character.");

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress().WithMessage("Invalid email address format.");


        }
    }
}
