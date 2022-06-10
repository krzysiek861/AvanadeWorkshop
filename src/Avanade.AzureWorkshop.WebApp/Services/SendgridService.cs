using Avanade.AzureWorkshop.WebApp.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Avanade.AzureWorkshop.WebApp.Services
{
    public class SendgridService
    {
        public async Task<HttpStatusCode> SendEmail()
        {
            var apiKey = GlobalSecrets.SendgridApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Test User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("test@example.com", "Test User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response.StatusCode;
        }
    }
}