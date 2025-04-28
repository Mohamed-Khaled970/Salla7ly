using Salla7ly.Infrastructure.Helpers;

namespace Salla7ly.Api.Helpers
{
    public class EmailBodyBuilder : IEmailBodyBuilder
    {
        public string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
        {
            var templatePath = $"{Directory.GetCurrentDirectory()}/EmailTemplates/{template}.html";
            var body = File.ReadAllText(templatePath);

            foreach (var item in templateModel)
                body = body.Replace(item.Key, item.Value);

            return body;
        }
    }
}
