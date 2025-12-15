using System.ComponentModel.DataAnnotations;

namespace BeatVault.API.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Artist"; // "Producer", "Artist", "Admin"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public List<Beat> Beats { get; set; } = new List<Beat>();
        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}