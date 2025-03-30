using MediatR;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Contracts
{
    public record SignInCommand
     (
        string Email,
        string Password
     ) : IRequest<Result<SignInCommandResponse>>;
}
