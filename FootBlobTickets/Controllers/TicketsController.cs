using Azure.Storage.Blobs;
using FootBlobTickets.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FootBlobTickets.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TicketsController : ControllerBase
	{
        public string? BlobContent { get; set; }

        // GET: api/<TicketsController>
        [HttpGet]
		public async Task<string> Get()
		{
			string? connString = Environment.GetEnvironmentVariable("local") ?? null;

			if (connString is not null)
			{
				BlobServiceClient blobServiceClient = new BlobServiceClient(connString);
				var blobContainerClient = blobServiceClient.GetBlobContainerClient("fixtures");
				await blobContainerClient.CreateIfNotExistsAsync();

				var blobClient = blobContainerClient.GetBlobClient("fixtures.txt");

				var response = await blobClient.DownloadAsync();

				using (var streamReader = new StreamReader(response.Value.Content))
				{
					BlobContent = await streamReader.ReadToEndAsync();
					return BlobContent;
				}
			}

			throw new Exception("No fixtures found.");
		}

		// POST api/<TicketsController> Ta emot fixture Id och antal biljetter
		[HttpPost]
		public async Task Post([FromBody] Ticket ticket)
		{
            string? connString = Environment.GetEnvironmentVariable("local") ?? null;
			
			if (connString is not null)
			{
				BlobServiceClient blobServiceClient = new BlobServiceClient(connString);
				var blobContainerClient = blobServiceClient.GetBlobContainerClient("sold-tickets");
				await blobContainerClient.CreateIfNotExistsAsync();

				string blobContent = JsonSerializer.Serialize(new
				{
					ticket.FixtureId,
					ticket.NumberOfTickets,
					ticket.Email
				});

				var blobClient = blobContainerClient.GetBlobClient($"sold-tickets-{Guid.NewGuid()}.txt");
				byte[] bytes = Encoding.UTF8.GetBytes(blobContent);

				using var stream = new MemoryStream(bytes);

				await blobClient.UploadAsync(stream, overwrite: true);
			}
		}
	}
}
