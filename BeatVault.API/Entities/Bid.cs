namespace BeatVault.API.Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User? User { get; set; }

        public int AuctionId { get; set; }
        public Auction? Auction { get; set; }
    }
}