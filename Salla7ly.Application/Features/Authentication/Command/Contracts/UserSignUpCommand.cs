using MediatR;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Contracts
{
    public record UserSignUpCommand
    (
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string Otp
    ) : IRequest<Result<SignInCommandResponse>>;
}
