using Avanade.AzureWorkshop.WebApp.Services;
using System.Threading.Tasks;

namespace Avanade.AzureWorkshop.WebApp.BusinessLogic
{
    public class NewsletterService
    {
        private readonly SendgridService _sendgridService;

        public NewsletterService(SendgridService sendgridService)
        {
            _sendgridService = sendgridService;
        }

        public async Task SendNewsletter()
        {
            await _sendgridService.SendEmail();
        }
    }
}