using System.Text;
using Azure.Identity;
using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;
using FootBlobTickets.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace func_update_fixtures
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task<string> Run([BlobTrigger("sold-tickets/{name}", Connection = "local")] Stream stream, string name)
        {
            // reading newly created ticket from blob storage, converting to ticket object
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            Ticket? ticket = JsonConvert.DeserializeObject<Ticket>(content);
            
            // sending get request to API, deserializing to list of fixtures
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://app-gladpack-projekt.azurewebsites.net/api/Tickets");
            string jsonContent = await response.Content.ReadAsStringAsync();
            List<Fixture> fixtures = JsonConvert.DeserializeObject<List<Fixture>>(jsonContent);

            // finding the matching fixture and updating TicketsSold property
            Fixture? fixtureToUpdate = fixtures.FirstOrDefault(f => f.FixtureId == ticket.FixtureId);
            fixtureToUpdate.TicketsSold += ticket.NumberOfTickets;

			string? connString = Environment.GetEnvironmentVariable("local") ?? null;

            if (connString is not null)
            {
			    BlobServiceClient blobServiceClient = new BlobServiceClient(connString);
			    var blobContainerClient = blobServiceClient.GetBlobContainerClient("fixtures");
			    
			    var blobClient = blobContainerClient.GetBlobClient("fixtures.txt");
                // serializing updated fixtures list
                var blobContent = System.Text.Json.JsonSerializer.Serialize(fixtures);
			    byte[] bytes = Encoding.UTF8.GetBytes(blobContent);

                // uploading updated list to blob storage
			    using var streamToUpdate = new MemoryStream(bytes);
			    await blobClient.UploadAsync(streamToUpdate, overwrite: true);

                // publishing event to event grid
                string endpoint = "https://evgt-gladpack.ukwest-1.eventgrid.azure.net/api/events";
                EventGridPublisherClient eventClient = new EventGridPublisherClient(
                    new Uri(endpoint),
                    new DefaultAzureCredential());

			    EventGridEvent egEvent =
				    new EventGridEvent("Example.EventSubject", "Example.EventType","1.0", ticket);

			    eventClient.SendEventAsync(egEvent).GetAwaiter().GetResult();
			
			    return blobContent;
            }

            throw new Exception("Something went wrong.");
		}
    }
}
