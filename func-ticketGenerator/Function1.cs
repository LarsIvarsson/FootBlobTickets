using System;
using Azure.Messaging;
using Azure.Messaging.ServiceBus;
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
        public async Task Run([EventGridTrigger] Azure.Messaging.EventGrid.EventGridEvent ev)
        {
            _logger.LogInformation("Event: {Content}", ev.Data.ToString());

            string connectionString = "Endpoint=sb://sb-gladpack.servicebus.windows.net/;SharedAccessKeyName=sas-send;SharedAccessKey=M2QghCLXP4E8Mu5hkAi1+SDowipEooFB0+ASbPJaX6E=;EntityPath=sbt-tickettopic";
            string topicName = "sbt-ticketTopic";
            ServiceBusClient sbClient = new(connectionString);
            ServiceBusSender sender = sbClient.CreateSender(topicName);

            string messageBody = $"Event: {ev.Data}";
            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(message);
        }
    }
}