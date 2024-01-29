using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;
using FootBlobTickets.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            Ticket? ticket = JsonConvert.DeserializeObject<Ticket>(content);
            
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://app-gladpack-projekt.azurewebsites.net/api/Tickets");

            string jsonContent = await response.Content.ReadAsStringAsync();
            List<Fixture> fixtures = JsonConvert.DeserializeObject<List<Fixture>>(jsonContent);

            Fixture? fixtureToUpdate = fixtures.FirstOrDefault(f => f.FixtureId == ticket.FixtureId);
            fixtureToUpdate.TicketsSold += ticket.NumberOfTickets;

			string connString = Environment.GetEnvironmentVariable("local") ?? "Hi-hi";

			BlobServiceClient blobServiceClient = new BlobServiceClient(connString);
			var blobContainerClient = blobServiceClient.GetBlobContainerClient("fixtures");

			string blobName = "fixtures.txt";
			var blobClient = blobContainerClient.GetBlobClient(blobName);

			//var fixturesResponse = await blobClient.DownloadAsync();
            var blobContent = JsonSerializer.Serialize(fixtures);

			byte[] bytes = Encoding.UTF8.GetBytes(blobContent);

			using var streamToUpdate = new MemoryStream(bytes);

			await blobClient.UploadAsync(streamToUpdate, overwrite: true);

            string endpoint = "https://evgt-gladpack.ukwest-1.eventgrid.azure.net/api/events";

            EventGridPublisherClient eventClient = new EventGridPublisherClient(new Uri(endpoint), new DefaultAzureCredential());

			EventGridEvent egEvent =
				new EventGridEvent(
					"ExampleEventSubject",
					"Example.EventType",
					"1.0",
					ticket
					);

			eventClient.SendEventAsync(egEvent).GetAwaiter().GetResult();
			

			return blobContent;

		}
    }
}
