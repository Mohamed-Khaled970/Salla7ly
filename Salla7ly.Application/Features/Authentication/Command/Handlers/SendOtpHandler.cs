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
using Salla7ly.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Salla7ly.Application.Features.Authentication.Command.Errors;
using Salla7ly.Application.Features.Authentication.Command.Responses;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class SendOtpHandler : IRequestHandler<SendOtpCommand,Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;
        private readonly IEmailService _emailService;

        public SendOtpHandler
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

        public async Task<Result> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExist)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.DublicatedEmail);

            var userNameIsExist = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);

            if (userNameIsExist)
                return Result.Failure<SignInCommandResponse>(AuthenticationErrors.DublicatedUserName);

            var otp = GenerateOTPNumber();
            var otpEntity = new UserOtp
            {
                Email = request.Email,
                Code = otp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(2)
            };

            _context.UserOtps.Add(otpEntity);
            await _context.SaveChangesAsync(cancellationToken);

            await SendOTPEmail(request.UserName, request.Email, otp);
            return Result.Success();
        }


        private string GenerateOTPNumber()
        {
            Random random = new Random();
            return random.Next(0, 10000000).ToString("D6");
        }
        private async Task SendOTPEmail(string userName, string Email, string OtpText)
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
    }
}

