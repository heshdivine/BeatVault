namespace BeatVault.API.DTOs
{
    public class BeatDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int BPM { get; set; }
        public string Key { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public int? AuctionId { get; set; }
        public decimal? LeasePrice { get; set; }

        // Flattening: We just want the name, not the whole User object
        public int ProducerId { get; set; }
        public string ProducerName { get; set; } = string.Empty;
    }
}