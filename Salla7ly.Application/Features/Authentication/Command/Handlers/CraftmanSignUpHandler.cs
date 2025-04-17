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

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class CraftmanSignUpHandler : IRequestHandler<CraftmanSignUpCommand, Result>
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
                await SendOTPEmail(user, otp);
                return Result.Success();
            }


            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
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


    }
}
