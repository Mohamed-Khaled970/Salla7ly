using MediatR;
using Microsoft.AspNetCore.Http;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Contracts
{
    public record CraftmanSignUpCommand
     (
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string DateOfBirth,
        string IdCardFrontUrl,
        string IdCardBackUrl,
        string Otp
     ) : IRequest<Result<SignInCommandResponse>>;
}
