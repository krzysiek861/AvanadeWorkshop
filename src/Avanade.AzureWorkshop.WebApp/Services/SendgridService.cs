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
            var from = new EmailAddress("mateusz.kruk@avanade.com", "Mateusz Kruk");
            var to = new EmailAddress("mateusz.kruk@windowslive.com", "Mateusz Kruk");

            var playerDetails = _playersService.GetRandomPlayerDetails();
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-4a3b06128b414dd1a4e1b047e0b33241",
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