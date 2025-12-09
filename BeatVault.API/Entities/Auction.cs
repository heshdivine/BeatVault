namespace BeatVault.API.Entities
{
    public class Auction
    {
        public int Id { get; set; }

        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Helper to check if bidding is allowed
        public bool IsActive => DateTime.UtcNow >= StartTime && DateTime.UtcNow < EndTime;

        // Relationship: 1 Auction belongs to 1 Beat
        public int BeatId { get; set; }
        public Beat? Beat { get; set; }

        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}