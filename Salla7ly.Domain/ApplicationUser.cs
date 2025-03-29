using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public DateOnly? DateOfBirth { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public string? IdCardFrontUrl { get; set; } = string.Empty;
        public string? IdCardBackUrl { get; set; } = string.Empty;
        public bool isDisabled { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
