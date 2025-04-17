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
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Salla7ly.Application.Features.Authentication.Command.Errors;
using Microsoft.AspNetCore.Http;
using Salla7ly.Infrastructure.Settings;
using Salla7ly.Infrastructure.Helpers;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using System.Security.Cryptography;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class CraftmanSignUpHandler : IRequestHandler<CraftmanSignUpCommand, Result<SignInCommandResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;
        private readonly IEmailService _emailService;

        public CraftmanSignUpHandler
                   (UserManager<ApplicationUser> userManager,
                    ApplicationDbContext context, IJwtProvider jwtProvider,
                    SignInManager<ApplicationUser> signInManager,
                    IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _jwtProvider = jwtProvider;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public async Task<Result<SignInCommandResponse>> Handle(CraftmanSignUpCommand request, CancellationToken cancellationToken)
        {
            var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExist)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.DublicatedEmail);

            var otp = await _context.UserOtps
                            .FirstOrDefaultAsync(x => x.Email == request.Email
                            && x.Code == request.Otp
                            && !x.IsDisabled
                            && x.ExpirationTime > DateTime.UtcNow,
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
                user.EmailConfirmed = true;

                await _context.SaveChangesAsync(cancellationToken);

                var userRole = await GetUserRole(user, cancellationToken);
                var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRole);

                var refreshToken = GenerateRefreshToken();
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


            var error = result.Errors.First();

            return Result.Failure<SignInCommandResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        private string GenerateOTPNumber()
        {
            Random random = new Random();
            return random.Next(0, 10000000).ToString("D6");
        }
        private async Task SendOTPEmail(ApplicationUser user, string OtpText)
        {
            var emailBody = EmailBodyBuilder.GenerateEmailBody("OtpTemplate",
                new Dictionary<string, string>
                {
                    { "{{Name}}" ,user.UserName! },
                    { "{{Otp}}" ,OtpText },
                    { "{{Year}}" ,"2025" }
                });

           await  _emailService.SendEmailAsync(user.Email!, "✅ Salla7ly: Email Confirmation", emailBody);
           await Task.CompletedTask;
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private async Task<string> GetUserRole(ApplicationUser user, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.FirstOrDefault()!;
        }
    }
}
