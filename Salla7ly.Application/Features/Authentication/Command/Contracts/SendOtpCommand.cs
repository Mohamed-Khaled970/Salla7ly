using MediatR;
using Salla7ly.Application.Common.Result_Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Contracts
{
    public record SendOtpCommand
    (
        string UserName,
        string Email
    ) : IRequest<Result>;
}
