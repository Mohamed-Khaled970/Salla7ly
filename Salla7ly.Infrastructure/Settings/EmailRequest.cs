using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Infrastructure.Settings
{
    public class EmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;

    }
}
