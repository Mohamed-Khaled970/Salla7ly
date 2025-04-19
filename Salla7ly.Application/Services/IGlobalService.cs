using Salla7ly.Application.Features.Authentication.Command.Responses;
using Salla7ly.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Services
{
    public interface IGlobalService
    {
        Task SendVerificationOtpAsync(string Email, string UserName, CancellationToken cancellationToken);
        Task SendForgetPasswordOtpAsync(string Email, CancellationToken cancellationToken);
        Task<string> GetUserRole(ApplicationUser user, CancellationToken cancellationToken);
        Task<Result<SignInCommandResponse>> GenerateSignUpToken(ApplicationUser user,UserOtp otp, CancellationToken cancellationToken);

        string GenerateRefreshToken();
    }

}
