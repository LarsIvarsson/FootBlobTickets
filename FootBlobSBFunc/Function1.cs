using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using FootBlobTickets.Entities;

namespace FootBlobSBFunc
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run(
            [ServiceBusTrigger("sbt-tickettopic", "sbs-ticketSubscription", Connection = "local")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            await messageActions.CompleteMessageAsync(message);

			string cosmosEndpointUrl = "https://gladpack-cosmosdb.documents.azure.com:443/";
            string? cosmosPrimaryKey = Environment.GetEnvironmentVariable("cosmos") ?? null;

            if (cosmosPrimaryKey is not null)
            {
                CosmosClient cosmosClient = new CosmosClient(cosmosEndpointUrl, cosmosPrimaryKey);
                DatabaseResponse dbResponse = await cosmosClient
                    .CreateDatabaseIfNotExistsAsync("footblobtickets-db");

                ContainerResponse cResponse = await dbResponse.Database
                    .CreateContainerIfNotExistsAsync("tickets", "/FixtureId");

                Ticket newTicket = message.Body.ToObjectFromJson<Ticket>();

                ItemResponse<Ticket> iResponse = await cResponse.Container
                    .CreateItemAsync(newTicket, new PartitionKey(newTicket.FixtureId.ToString()));
            }
        }
    }
}