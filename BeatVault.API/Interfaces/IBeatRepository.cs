using BeatVault.API.Entities;

namespace BeatVault.API.Interfaces
{
    public interface IBeatRepository
    {
        // 1. Get all beats (Async to not block the thread)
        Task<IEnumerable<Beat>> GetAllBeatsAsync();

        // 2. Get a single beat by ID
        Task<Beat?> GetBeatByIdAsync(int id);

        // 3. Get all beats by a specific producer (Filtering)
        Task<IEnumerable<Beat>> GetBeatsByProducerAsync(int producerId);

        // 4. Add a new beat
        Task AddBeatAsync(Beat beat);

        // 5. Save changes to the database
        Task<bool> SaveChangesAsync();
    }
}