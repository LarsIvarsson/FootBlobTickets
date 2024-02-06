namespace FootBlobTickets.Entities
{
	public class Ticket
	{
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid FixtureId { get; set; }
        public int NumberOfTickets { get; set; }
        public string Email { get; set; }
    }
}
