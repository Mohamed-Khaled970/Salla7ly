using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Infrastructure.Helpers
{
    public class EmailBodyBuilder
    {
        public static string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
        {

            var currentDirectory = Directory.GetCurrentDirectory();
 
            var solutionRoot = Directory.GetParent(currentDirectory)?.FullName;

            if (solutionRoot is null)
                throw new DirectoryNotFoundException("Cannot locate solution root directory.");

            var templatePath = Path.Combine(
                solutionRoot,
                "Salla7ly.Infrastructure",
                "Email Templates",
                $"{template}.html"
            );

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Email template not found at path: {templatePath}");

            var body = File.ReadAllText(templatePath);

            foreach (var item in templateModel)
                body = body.Replace(item.Key, item.Value);

            return body;
        }

    }
}
