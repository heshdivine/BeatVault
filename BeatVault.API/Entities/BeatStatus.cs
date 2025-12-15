namespace BeatVault.API.Entities
{
    public enum BeatStatus
    {
        Available,       // Open for Leases and/or Auction
        PendingPayment,  // Auction won, waiting for transfer
        Sold             // Exclusive rights sold (Locked)
    }
}