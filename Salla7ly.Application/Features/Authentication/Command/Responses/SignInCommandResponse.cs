﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Responses
{
    public record SignInCommandResponse(
         string Id,
         string? Email,
         string? FirstName,
         string? LastName,
         string Token,
         int ExpiresIn,
         string RefreshToken,
         DateTime RefreshTokenExpiration
    );
}
