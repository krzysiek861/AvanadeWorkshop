using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Threading.Tasks;

namespace Avanade.AzureWorkshop.WebApp.Services
{
    public class MailgunService
    {
        public async Task<HttpStatusCode> SendEmail()
        {
            var client = new RestClient("https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api",
                    "key-137c11c4021afffb03433a088e2d03bd")
            };

            var request = new RestRequest();
            request.AddParameter("domain", "sandboxc0852d6e6a1c4f2685f9333b377bdb28.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxc0852d6e6a1c4f2685f9333b377bdb28.mailgun.org>");
            request.AddParameter("to", "example@gmail.com");
            request.AddParameter("subject", "World Cup Newsletter");
            request.AddParameter("text", "Daily Summary");
            request.Method = Method.Post;

            var response = await client.ExecuteAsync(request);

            return response.StatusCode;
        }
    }
}