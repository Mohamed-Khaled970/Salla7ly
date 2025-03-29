using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Infrastructure.Authentication
{
    public class JwtOptions
    {
        public static string SectionName = "Jwt";

        [Required]
        public string Key { get; init; } = string.Empty;
        [Required]
        public string Issuer { get; init; } = string.Empty;
        [Required]
        public string Audience { get; init; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Range for Expire Date")]
        public int ExpiryMinutes { get; init; }


    }
}
