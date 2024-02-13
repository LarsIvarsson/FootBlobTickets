using Azure.Messaging.ServiceBus;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace func_ticketGenerator
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run([EventGridTrigger] EventGridEvent ev)
        {
            _logger.LogInformation("Event: {Content}", ev.Data.ToString());

            string? connString = Environment.GetEnvironmentVariable("local") ?? null;

            if (connString is not null)
            {
                ServiceBusClient sbClient = new(connString);
                ServiceBusSender sender = sbClient.CreateSender("sbt-ticketTopic");

                string messageBody = $"{ev.Data}";
                ServiceBusMessage message = new ServiceBusMessage(messageBody);
                await sender.SendMessageAsync(message);
            }            
        }
    }
}