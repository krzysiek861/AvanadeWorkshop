using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Avanade.AzureWorkshop.Topics
{
    internal class Program
    {
        public static IContainer Container { get; private set; }

        static async Task Main()
        { 
            var container = new IocConfig();
            Container = container.GetConfiguredContainer();

            var builder = new HostBuilder();
            builder.UseEnvironment(Environments.Development);
            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddServiceBus();
            });
            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
