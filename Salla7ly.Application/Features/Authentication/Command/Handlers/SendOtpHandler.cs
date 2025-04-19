using MediatR;
using Microsoft.AspNetCore.Identity;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using Salla7ly.Domain;
using Salla7ly.Infrastructure.Authentication;
using Salla7ly.Infrastructure.Services;
using Salla7ly.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Salla7ly.Application.Features.Authentication.Command.Errors;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using Salla7ly.Application.Services;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class SendOtpHandler : IRequestHandler<SendOtpCommand,Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGlobalService _globalService;

        public SendOtpHandler
                   (UserManager<ApplicationUser> userManager,
                     IGlobalService globalService)
        {
            _userManager = userManager;
            _globalService = globalService; 
        }

        public async Task<Result> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExist)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.DublicatedEmail);

            var userNameIsExist = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);

            if (userNameIsExist)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.DublicatedUserName);

          await _globalService.SendOtpAsync(request.Email, request.UserName, cancellationToken); 

            return Result.Success();
        }


    }
}

