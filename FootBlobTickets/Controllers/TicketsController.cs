using Azure.Storage.Blobs;
using FootBlobTickets.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FootBlobTickets.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TicketsController : ControllerBase
	{
		// GET: api/<TicketsController>
		[HttpGet]
		public async Task<string> Get()
		{
			string connString = Environment.GetEnvironmentVariable("local") ?? "M.I.A";
			string blobContent;

			BlobServiceClient blobServiceClient = new BlobServiceClient(connString);
			var blobContainerClient = blobServiceClient.GetBlobContainerClient("fixtures");
			await blobContainerClient.CreateIfNotExistsAsync();

			string blobName = "fixtures.txt";
			var blobClient = blobContainerClient.GetBlobClient(blobName);

			var response = await blobClient.DownloadAsync();

			using (var streamReader = new StreamReader(response.Value.Content))
			{
				blobContent = await streamReader.ReadToEndAsync();
			}

			return blobContent;
		}

		// GET api/<TicketsController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<TicketsController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<TicketsController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<TicketsController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
