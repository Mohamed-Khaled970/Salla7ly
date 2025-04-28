using MediatR;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Contracts
{
    public record GoogleSignInCommand
    (
        string Email,
        string Name
    ) : IRequest<Result<SignInCommandResponse>>;
}
