using System.ComponentModel.DataAnnotations.Schema;

namespace BeatVault.API.Entities
{
    public class Beat
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int BPM { get; set; }
        public string Key { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;

        // HYBRID LOGIC:
        // If this has a value (e.g., 29.99), artists can buy a lease instantly.
        public decimal? LeasePrice { get; set; }

        public BeatStatus Status { get; set; } = BeatStatus.Available;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        // Relationship: The Producer
        public int ProducerId { get; set; }
        public User? Producer { get; set; }

        // Relationship: The Exclusive Auction (Optional)
        // If this is null, the beat is ONLY for lease.
        public Auction? Auction { get; set; }
    }
}