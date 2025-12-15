using BeatVault.API.Data;
using BeatVault.API.Entities;
using BeatVault.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeatVault.API.Data.Repositories
{
    public class BeatRepository : IBeatRepository
    {
        private readonly DataContext _context;

        // Dependency Injection: We ask for the DataContext here
        public BeatRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Beat>> GetAllBeatsAsync()
        {
            // We include the Producer (User) so we don't just get a null ID.
            // This is called "Eager Loading".
            return await _context.Beats
                .Include(b => b.Producer)
                .Include(b => b.Auction)
                .OrderByDescending(b => b.UploadedDate)
                .ToListAsync();
        }

        public async Task<Beat?> GetBeatByIdAsync(int id)
        {
            return await _context.Beats
                .Include(b => b.Producer)
                .Include(b => b.Auction)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Beat>> GetBeatsByProducerAsync(int producerId)
        {
            return await _context.Beats
                .Where(b => b.ProducerId == producerId)
                .ToListAsync();
        }

        public async Task AddBeatAsync(Beat beat)
        {
            await _context.Beats.AddAsync(beat);
        }

        public async Task<bool> SaveChangesAsync()
        {
            // Returns true if 1 or more changes were saved
            return await _context.SaveChangesAsync() > 0;
        }
    }
}