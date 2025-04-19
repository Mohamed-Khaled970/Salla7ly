using Microsoft.AspNetCore.Identity;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using Salla7ly.Domain;
using Salla7ly.Infrastructure;
using Salla7ly.Infrastructure.Authentication;
using Salla7ly.Infrastructure.Helpers;
using Salla7ly.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Salla7ly.Application.Services
{
    public class GlobalService(IEmailService emailService ,
                    ApplicationDbContext context,
                    UserManager<ApplicationUser> userManager,
                    IJwtProvider jwtProvider) : IGlobalService
    {

        private readonly IEmailService _emailService = emailService;
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;
        public async Task SendVerificationOtpAsync(string Email, string UserName, CancellationToken cancellationToken)
        {
            var otp = GenerateOTPNumber();
            var otpEntity = new UserOtp
            {
                Email = Email,
                Code = otp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(2)
            };

            _context.UserOtps.Add(otpEntity);
            await _context.SaveChangesAsync(cancellationToken);

            await SendVerificationOTPEmail(UserName, Email, otp);
        }

        public async Task SendForgetPasswordOtpAsync(string Email, CancellationToken cancellationToken)
        {
            var otp = GenerateOTPNumber();
            var otpEntity = new UserOtp
            {
                Email = Email,
                Code = otp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(2)
            };

            _context.UserOtps.Add(otpEntity);
            await _context.SaveChangesAsync(cancellationToken);

            await SendForgetPasswordOTPEmail(Email, otp);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<string> GetUserRole(ApplicationUser user, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.FirstOrDefault()!;
        }

        public async Task<Result<SignInCommandResponse>> GenerateSignUpToken(ApplicationUser user, UserOtp otp, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = true;
            otp.IsDisabled = true;
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

        private string GenerateOTPNumber()
        {
            Random random = new Random();
            return random.Next(0, 10000000).ToString("D6");
        }
        private async Task SendVerificationOTPEmail(string userName, string Email, string OtpText)
        {
            var emailBody = EmailBodyBuilder.GenerateEmailBody("OtpTemplate",
                new Dictionary<string, string>
                {
                    { "{{Name}}" ,userName! },
                    { "{{Otp}}" ,OtpText },
                    { "{{Year}}" ,"2025" }
                });

            await _emailService.SendEmailAsync(Email, "✅ Salla7ly: Email Confirmation", emailBody);
            await Task.CompletedTask;
        }

        private async Task SendForgetPasswordOTPEmail(string Email, string OtpText)
        {
            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPasswordOtpTemplate",
                new Dictionary<string, string>
                {
                    { "{{Otp}}" ,OtpText },
                    { "{{Year}}" ,"2025" }
                });

            await _emailService.SendEmailAsync(Email, "✅ Salla7ly: Reset Password", emailBody);
            await Task.CompletedTask;
        }


    }
}
