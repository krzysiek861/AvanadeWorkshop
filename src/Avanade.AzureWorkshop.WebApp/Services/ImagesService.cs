using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Avanade.AzureWorkshop.WebApp.Services
{
    public class ImagesService
    {
        const string pattern = "\"contentUrl\": \"(.*?)\"";
        const int maxMatches = 3;

        private string accessKey = ConfigurationManager.AppSettings["bingImagesSearchKey"];
        const string uriBase = @"https://api.bing.microsoft.com/v7.0/images/search";

        public IEnumerable<string> SearchForImages(string searchQuery)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", accessKey);
            var builder = new UriBuilder(uriBase);
            builder.Query = $"q={Uri.EscapeDataString(searchQuery)}";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var response = httpClient.GetAsync(builder.Uri).Result;

            var json = new StreamReader(response.Content.ReadAsStreamAsync().Result).ReadToEnd();

            int i = 0;
            foreach (Match match in Regex.Matches(json, pattern))
            {
                if (i++ == maxMatches) break;
                yield return match.Groups[1].Value;
            }
        }

        public byte[] DownloadImage(string url)
        {
            return new WebClient().DownloadData(url.Replace(@"\/", @"/"));
        }
    }
}