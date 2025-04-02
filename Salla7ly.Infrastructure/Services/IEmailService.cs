using Salla7ly.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendEmail(EmailRequest request);
    }
}
