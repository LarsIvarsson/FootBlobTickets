using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using FootBlobTickets.Entities;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

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

             // Complete the message
            await messageActions.CompleteMessageAsync(message);

			string cosmosEndpointUrl = "https://gladpack-cosmosdb.documents.azure.com:443/";
			string cosmosPrimaryKey = "r3cB3jT9GUcZLoDoZXmP93xJV6Ig33U25YM6OuyBwFbELwkkzn5iPYUomXnhYocdU3zo3wwXqfByACDbKx2z3g==";

            CosmosClient cosmosClient = new CosmosClient(cosmosEndpointUrl, cosmosPrimaryKey);
            DatabaseResponse dbResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync("footblobtickets-db");

            Console.WriteLine($"Found or created db with id: {dbResponse.Database.Id}");

            ContainerResponse cResponse = await dbResponse.Database.CreateContainerIfNotExistsAsync("tickets", "/FixtureId");
            Console.WriteLine($"Found or created container with id: {cResponse.Container.Id}");

            Ticket newTicket = message.Body.ToObjectFromJson<Ticket>();

            ItemResponse<Ticket> iResponse = await cResponse.Container.CreateItemAsync(newTicket, new PartitionKey(newTicket.FixtureId.ToString()));
            Console.WriteLine("We made it...!?");
        }
    }
}
