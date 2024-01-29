// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Azure.Messaging;
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
        public void Run([EventGridTrigger] Azure.Messaging.EventGrid.EventGridEvent ev)
        {
            _logger.LogInformation("Event type: {type}, Event subject: {subject}", ev.EventType, ev.Subject, ev.Data.ToString());
        }
    }
}
