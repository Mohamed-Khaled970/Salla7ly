using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using Salla7ly.Application.Features.Authentication.Command.Errors;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using Salla7ly.Application.Services;
using Salla7ly.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class SendForgetPasswordOtpHandler : IRequestHandler<SendForgetPasswordOtpCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGlobalService _globalService;

        public SendForgetPasswordOtpHandler
                   (UserManager<ApplicationUser> userManager,
                     IGlobalService globalService)
        {
            _userManager = userManager;
            _globalService = globalService;
        }

        public async Task<Result> Handle(SendForgetPasswordOtpCommand request, CancellationToken cancellationToken)
        {
            var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (!emailIsExist)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.UserEmailNotFound);

            await _globalService.SendForgetPasswordOtpAsync(request.Email, cancellationToken);

            return Result.Success();
        }
    }
}
