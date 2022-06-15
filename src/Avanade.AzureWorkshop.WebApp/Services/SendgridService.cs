using Avanade.AzureWorkshop.WebApp.BusinessLogic;
using Avanade.AzureWorkshop.WebApp.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Avanade.AzureWorkshop.WebApp.Services
{
    public class SendgridService
    {
        private readonly PlayersService _playersService;

        public SendgridService(PlayersService playersService)
        {
            _playersService = playersService;
        }

        public async Task<HttpStatusCode> SendEmail()
        {
            var apiKey = GlobalSecrets.SendgridApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("from@example.com", "From Name");
            var to = new EmailAddress("to@example.com", "To Name");
            var templateId = "";

            var playerDetails = _playersService.GetRandomPlayerDetails();
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId,
                new
                {
                    fullName = playerDetails.FullName,
                    imageUrl = playerDetails.Images.FirstOrDefault(),
                    position = playerDetails.Position,
                    number = playerDetails.Number
                });
            msg.Subject = $"Player Card: {playerDetails.FullName}";

            var response = await client.SendEmailAsync(msg);

            return response.StatusCode;
        }
    }
}