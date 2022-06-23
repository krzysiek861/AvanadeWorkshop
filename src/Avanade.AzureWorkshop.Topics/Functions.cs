using Autofac;
using Avanade.AzureWorkshop.WebApp.BusinessLogic;
using Avanade.AzureWorkshop.WebApp.Models.ServiceBusModels;
using Avanade.AzureWorkshop.WebApp.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Avanade.AzureWorkshop.Topics
{
    public class Functions
    {
        private const string SubscriptionName = "webjobssubscription";

        public async Task ProcessGameMessage([ServiceBusTrigger(nameof(GameMessageModel), SubscriptionName, Connection = "AzureWebJobsServiceBus")] GameMessageModel message, TextWriter textWriter)
        {
            await ProcessMessage(textWriter, message, async (scope, model) =>
            {
                var gamesService = scope.Resolve<GamesService>();
                var telemetryService = scope.Resolve<TelemetryService>();
                await gamesService.SaveGameResult(message);
                await WriteMessage(message.CorrelationId, textWriter);
                telemetryService.Log("Succesfully saved game result", message.CorrelationId);
            });
        }

        private static async Task ProcessMessage<TMessage>(TextWriter textWriter, TMessage message, Func<ILifetimeScope, TMessage, Task> action)
            where TMessage : BaseMessageModel
        {
            using (var scope = Program.Container.BeginLifetimeScope())
            {
                await WriteMessage($"Processing topic message {typeof(TMessage).Name}. Body: {JsonConvert.SerializeObject(message)}", textWriter);

                try
                {
                    await action(scope, message);
                }
                catch (Exception ex)
                {
                    textWriter.WriteLine($"Unexpected error {ex.Message} {ex.StackTrace} {ex.InnerException}");
                    throw;
                }

            }
        }

        private static async Task WriteMessage(string message, TextWriter writer)
        {
            await writer.WriteLineAsync(message);
        }
    }
}
