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

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class AuthenticationCommandHandler :
        IRequestHandler<SignInCommand, Result<SignInCommandResponse>> , IRequestHandler<CraftmanSignUpCommand , Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;
        private readonly IEmailService _emailService;

        public AuthenticationCommandHandler
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


        #region Login 
        
        public async Task<Result<SignInCommandResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.InvalidCredentails);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);


            if (result.Succeeded)
            {
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

                var response = new SignInCommandResponse(user.Id, user.Email,user.UserName, token, expiresIn, refreshToken, refreshTokenExpiration);

                return Result.Success(response);
            }

            return Result.Failure<SignInCommandResponse>(result.IsNotAllowed ? AuthenticationErrors.EmailNotConfirmed : AuthenticationErrors.InvalidCredentails);
        }
        #endregion

        #region Craft-man SignUp
        
        public async Task<Result> Handle(CraftmanSignUpCommand request, CancellationToken cancellationToken)
        {
            var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExist)
                return Result.Failure(AuthenticationErrors.DublicatedEmail);

            var user = request.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var otp = GenerateOTPNumber();
                await SendOTPEmail(user.Email!, otp, user.UserName!);
                return Result.Success();
            }


            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        #endregion
        private async Task<string> GetUserRole(ApplicationUser user, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.FirstOrDefault()!;
        }

        private async Task SendOTPEmail(string useremail, string OtpText , string Name)
        {
            var mailRequest = new EmailRequest();
            mailRequest.Email = useremail;
            mailRequest.Subject = "Thanks for registering  : OTP";
            mailRequest.EmailBody = GenerateEmailBody(Name, OtpText);
            await _emailService.SendEmail(mailRequest);
        }

        private string GenerateEmailBody(string name, string otpText)
        {
            string emailBody = $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>OTP Verification</title>
        <style>
            body {{
                margin: 0;
                padding: 0;
                font-family: 'Helvetica Neue', Arial, sans-serif;
                background-color: #f4f4f9;
            }}
            .container {{
                max-width: 600px;
                margin: 30px auto;
                background-color: #ffffff;
                border-radius: 12px;
                overflow: hidden;
                box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
            }}
            .header {{
                background: linear-gradient(135deg, #6b48ff 0%, #00ddeb 100%);
                padding: 30px;
                text-align: center;
                color: #ffffff;
            }}
            .header h1 {{
                margin: 0;
                font-size: 28px;
                font-weight: 700;
            }}
            .content {{
                padding: 40px 30px;
                text-align: center;
                color: #333333;
            }}
            .content p {{
                font-size: 16px;
                line-height: 1.6;
                margin: 0 0 20px;
            }}
            .otp-box {{
                display: inline-block;
                background-color: #f0f0ff;
                color: #6b48ff;
                font-size: 24px;
                font-weight: bold;
                padding: 15px 25px;
                border-radius: 8px;
                letter-spacing: 2px;
                margin: 20px 0;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
            }}
            .content .highlight {{
                color: #6b48ff;
                font-weight: 600;
            }}
            .footer {{
                background-color: #f8f8f8;
                padding: 20px;
                text-align: center;
                font-size: 12px;
                color: #777777;
                border-top: 1px solid #e8e8e8;
            }}
            .footer a {{
                color: #6b48ff;
                text-decoration: none;
            }}
            @media only screen and (max-width: 600px) {{
                .container {{
                    margin: 10px;
                    border-radius: 8px;
                }}
                .header h1 {{
                    font-size: 24px;
                }}
                .content {{
                    padding: 20px;
                }}
                .otp-box {{
                    font-size: 20px;
                    padding: 10px 20px;
                }}
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>Verify Your Account</h1>
            </div>
            <div class='content'>
                <p>Hello <span class='highlight'>{name}</span>,</p>
                <p>Thank you for joining us! To complete your registration, please use the One-Time Password (OTP) below:</p>
                <div class='otp-box'>{otpText}</div>
                <p>This OTP is valid for the next 10 minutes. If you didn’t request this, please ignore this email or contact our support team.</p>
            </div>
            <div class='footer'>
                <p>&copy; {DateTime.Now.Year} Salla7ly. All rights reserved.</p>
                <p>Need help? <a href='mailto:support@salla7ly.com'>Contact Support</a></p>
            </div>
        </div>
    </body>
    </html>";

            return emailBody;
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string GenerateOTPNumber()
        {
            Random random = new Random();
            return random.Next(0,10000000).ToString("D6");
        }


    }
}
