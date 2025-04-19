using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using Salla7ly.Domain;
using Salla7ly.Infrastructure.Authentication;
using Salla7ly.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Errors;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Salla7ly.Infrastructure.Services;
using Salla7ly.Infrastructure.Settings;
using Salla7ly.Application.Services;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class SignInHandler :
        IRequestHandler<SignInCommand, Result<SignInCommandResponse>> 
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;
        private readonly IEmailService _emailService;
        private readonly IGlobalService _globalService;

        public SignInHandler
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
        public async Task<Result<SignInCommandResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.InvalidCredentails);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);


            if (result.Succeeded)
            {
                var userRole = await _globalService.GetUserRole(user, cancellationToken);
                var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRole);

                var refreshToken = _globalService.GenerateRefreshToken();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpiration
                });

                await _userManager.UpdateAsync(user);

                var response = new SignInCommandResponse(user.Id, user.Email,user.UserName, token, expiresIn, refreshToken, refreshTokenExpiration);

                return Result.Success(response);
            }

            return Result.Failure<SignInCommandResponse>(result.IsNotAllowed ? AuthenticationErrors.EmailNotConfirmed : AuthenticationErrors.InvalidCredentails);
        }



    }
}
