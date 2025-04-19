using FluentValidation;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Validators
{
    public class SendVreficationOtpCommandValidator : AbstractValidator<SendVreficationOtpCommand>
    {
        public SendVreficationOtpCommandValidator()
        {
            RuleFor(x => x.UserName)
                    .NotNull()
                    .NotEmpty()
                    .Matches(@"^[a-zA-Z]{3}[a-zA-Z0-9_]{2,17}$")
                    .WithMessage("UserName must be 5 to 20 characters long, start with at least 3 letters, and can include letters, numbers, and underscores.");

            RuleFor(x => x.Email)
                    .NotNull()
                    .NotEmpty()
                    .EmailAddress().WithMessage("Invalid email address format.");

        }
    }
}
