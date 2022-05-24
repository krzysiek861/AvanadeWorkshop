using Autofac;
using Avanade.AzureWorkshop.WebApp.Models;
using Avanade.AzureWorkshop.WebApp.Services;
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

            var delayedConfigurationSource = new DelayedConfigurationSource();

            var builder = new HostBuilder()
                .ConfigureHostConfiguration(configurationBuilder =>
                {
                    configurationBuilder.Add(delayedConfigurationSource);
                });
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
                var manager = new SecretsManager();
                manager.Initialize();

                delayedConfigurationSource.Set("ConnectionStrings:AzureWebJobsServiceBus", GlobalSecrets.ServiceBusConnectionString);
                delayedConfigurationSource.Set("ConnectionStrings:AzureWebJobsDashboard", GlobalSecrets.StorageAccountConnectionString);
                delayedConfigurationSource.Set("ConnectionStrings:AzureWebJobsStorage", GlobalSecrets.StorageAccountConnectionString);

                await host.RunAsync().ConfigureAwait(false);
            }
        }
    }
}
