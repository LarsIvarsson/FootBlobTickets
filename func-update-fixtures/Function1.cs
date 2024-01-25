using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
        public async Task Run([BlobTrigger("sold-tickets/{name}", Connection = "local")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            Ticket ticket = JsonConvert.DeserializeObject<Ticket>(content);
            
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://app-gladpack-projekt.azurewebsites.net/api/Tickets");

            string jsonContent = await response.Content.ReadAsStringAsync();
            List<Fixture> fixtures = JsonConvert.DeserializeObject<List<Fixture>>(jsonContent);

            foreach (Fixture fixture in fixtures)
            {
                if (fixture.FixtureId == ticket.FixtureId)
                {
                    _logger.LogInformation($"{fixture.HomeTeam} - {fixture.AwayTeam}");
                }
            }

            _logger.LogInformation(fixtures[1].HomeTeam);
        }
    }
}
