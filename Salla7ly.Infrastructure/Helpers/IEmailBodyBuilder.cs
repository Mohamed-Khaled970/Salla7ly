using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Infrastructure.Helpers
{
    public interface IEmailBodyBuilder
    {
        string GenerateEmailBody(string template, Dictionary<string, string> templateModel);
    }
}
