using BeatVault.API.Entities;

namespace BeatVault.API.Interfaces
{
    public interface IAuctionRepository
    {
        Task<Auction?> GetAuctionByBeatIdAsync(int beatId);
        Task AddBidAsync(Bid bid);
        Task UpdateAuctionAsync(Auction auction); // To update CurrentPrice
        Task<bool> SaveChangesAsync();
    }
}