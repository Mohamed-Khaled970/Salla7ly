using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Salla7ly.Application.Common.Result_Pattern;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using Salla7ly.Application.Features.Authentication.Command.Errors;
using Salla7ly.Application.Features.Authentication.Command.Responses;
using Salla7ly.Application.Services;
using Salla7ly.Domain;
using Salla7ly.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class ValidateForgePasswordOtpHandler : IRequestHandler<ValidateForgePasswordOtpCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGlobalService _globalService;
        private readonly ApplicationDbContext _context;
        public ValidateForgePasswordOtpHandler
                   (UserManager<ApplicationUser> userManager,
                     IGlobalService globalService,
                     ApplicationDbContext context)
        {
            _userManager = userManager;
            _globalService = globalService;
            _context = context;
        }

        public async Task<Result> Handle(ValidateForgePasswordOtpCommand request, CancellationToken cancellationToken)
        {
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

                return Result.Failure(AuthenticationErrors.InvalidOtp);
            }

            return Result.Success();
        }
    }
}
