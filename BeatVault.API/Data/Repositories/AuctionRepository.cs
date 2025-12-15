using BeatVault.API.Data;
using BeatVault.API.Entities;
using BeatVault.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeatVault.API.Data.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly DataContext _context;

        public AuctionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Auction?> GetAuctionByBeatIdAsync(int beatId)
        {
            return await _context.Auctions
                .Include(a => a.Bids) // Load history
                .FirstOrDefaultAsync(a => a.BeatId == beatId);
        }

        public async Task AddBidAsync(Bid bid)
        {
            await _context.Bids.AddAsync(bid);
        }

        public async Task UpdateAuctionAsync(Auction auction)
        {
            // EF Core tracks changes automatically, but explicit update is safe
            _context.Auctions.Update(auction);
            await Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}