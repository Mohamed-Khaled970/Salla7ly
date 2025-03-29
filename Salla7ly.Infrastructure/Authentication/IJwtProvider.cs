using Salla7ly.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Infrastructure.Authentication
{
    public interface IJwtProvider
    {
        (string token, int expireTime) GenerateToken(ApplicationUser user, string role);
        string? ValidateToken(string token);
    }
}
