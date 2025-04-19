﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using Salla7ly.Application.Features.Authentication.Command.Errors;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using Salla7ly.Domain;
using Salla7ly.Infrastructure.Authentication;
using Salla7ly.Infrastructure.Helpers;
using Salla7ly.Infrastructure.Services;
using Salla7ly.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mapster;
using Salla7ly.Application.Services;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class UserSignUpHandler : IRequestHandler<UserSignUpCommand, Result<SignInCommandResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;
        private readonly IEmailService _emailService;
        private readonly IGlobalService _globalService;

        public UserSignUpHandler
                   (UserManager<ApplicationUser> userManager,
                    ApplicationDbContext context, IJwtProvider jwtProvider,
                    SignInManager<ApplicationUser> signInManager,
                    IEmailService emailService,
                    IGlobalService globalService)
        {
            _userManager = userManager;
            _context = context;
            _jwtProvider = jwtProvider;
            _signInManager = signInManager;
            _emailService = emailService;
            _globalService = globalService;
        }
        public async Task<Result<SignInCommandResponse>> Handle(UserSignUpCommand request, CancellationToken cancellationToken)
        {
            var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExist)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.DublicatedEmail);

            var userNameIsExist = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);

            if (userNameIsExist)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.DublicatedUserName);

            var otp = await _context.UserOtps
                            .FirstOrDefaultAsync(x => x.Email == request.Email
                            && x.Code == request.Otp
                            && !x.IsDisabled,
                            cancellationToken);

            if (otp is null || otp.IsDisabled || otp.ExpirationTime < DateTime.UtcNow)
            {
                if (otp is not null && !otp.IsDisabled && otp.ExpirationTime < DateTime.UtcNow)
                {
                    otp.IsDisabled = true;
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.InvalidOtp);
            }


            var user = request.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return await _globalService.GenerateSignUpToken(user, otp, cancellationToken);
            }

            var error = result.Errors.First();

            return Result.Failure<SignInCommandResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

    }
}
