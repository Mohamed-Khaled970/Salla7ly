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

namespace Salla7ly.Application.Features.Authentication.Command.Handlers
{
    public class AuthenticationCommandHanlder : IRequestHandler<SignInCommand, Result<SignInCommandResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        private readonly int _refreshTokenExpiryDays = 7;

        public AuthenticationCommandHanlder
                   (UserManager<ApplicationUser> userManager,
                    ApplicationDbContext context, IJwtProvider jwtProvider,
                    SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _jwtProvider = jwtProvider;
            _signInManager = signInManager;
        }
        public Task<Result<SignInCommandResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
