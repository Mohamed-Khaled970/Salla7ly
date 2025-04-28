using MediatR;
using Microsoft.AspNetCore.Identity;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using Salla7ly.Application.Services;
using Salla7ly.Domain;
using Salla7ly.Infrastructure.Authentication;
using Salla7ly.Infrastructure.Services;
using Salla7ly.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Errors;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class RefreshTokenHandler :
        IRequestHandler<RefreshTokenCommand, Result<SignInCommandResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;
        private readonly IEmailService _emailService;
        private readonly IGlobalService _globalService;

        public RefreshTokenHandler
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

        public async Task<Result<SignInCommandResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            string token = request.Token;
            string refreshToken = request.RefreshToken;

            // get user id by ValidateToken
            var userId = _jwtProvider.ValidateToken(token);

            if (userId is null)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.InvalidJwtToken);
            // select user from database
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.InvalidJwtToken);

            //check if the user is the owner of the refresh token and refresh token is acitve
            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);


            // check if El Helaly made a class for refresh token errors
            if (userRefreshToken is null)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.InvalidRefreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var userRoles = await _globalService.GetUserRole(user, cancellationToken);

            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);
            var newRefreshToken = _globalService.GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new SignInCommandResponse(user.Id, user.Email, user.UserName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }
    }
}
