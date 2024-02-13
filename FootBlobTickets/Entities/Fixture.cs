namespace FootBlobTickets.Entities
{
	public class Fixture
	{
        public Guid FixtureId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Stadium { get; set; }
        public int StadiumCapacity { get; set; }
        public int TicketsSold { get; set; }
    }
}
