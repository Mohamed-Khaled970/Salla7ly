using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Domain
{
    public class UserOtp
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Code { get; set; } = null!;
        public DateTime ExpirationTime { get; set; }

        public bool IsDisabled { get; set; } = false;
    }
}
