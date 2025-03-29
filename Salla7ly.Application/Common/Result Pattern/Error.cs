using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Common.Result_Pattern
{
    public record Error(string Code, string Description, int? StatusCode)
    {
        public static readonly Error None = new(string.Empty, string.Empty, null);
    }
}
