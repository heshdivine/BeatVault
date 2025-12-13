using System.ComponentModel.DataAnnotations;

namespace BeatVault.API.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; } // Who bought it
        public User? User { get; set; }

        public int BeatId { get; set; } // What they bought
        public Beat? Beat { get; set; }

        public decimal PricePaid { get; set; } // Actual amount charged
        public string StripePaymentIntentId { get; set; } = string.Empty; // To track refunds if needed

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}