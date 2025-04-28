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
using Microsoft.EntityFrameworkCore;
using Mapster;
using Salla7ly.Application.Common.Result_Pattern;
using System.Threading;
using Salla7ly.Infrastructure.Consts;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class GoogleSignInHandler :
        IRequestHandler<GoogleSignInCommand, Result<SignInCommandResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;
        private readonly IEmailService _emailService;
        private readonly IGlobalService _globalService;

        public GoogleSignInHandler
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
        public async Task<Result<SignInCommandResponse>> Handle(GoogleSignInCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user is null)
            {
                var createdUser = request.Adapt<ApplicationUser>();

                var result = await _userManager.CreateAsync(user!);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, DefaultRoles.User);
                    return await GetTokenAndRefreshToken(user! , cancellationToken);
                }
            }

            return await GetTokenAndRefreshToken(user!, cancellationToken);
        }


        private async Task<Result<SignInCommandResponse>> GetTokenAndRefreshToken(ApplicationUser user , CancellationToken cancellationToken)
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

            var response = new SignInCommandResponse(user.Id, user.Email, user.UserName, token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }
    }

}
