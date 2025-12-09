using BeatVault.API.Entities;

namespace BeatVault.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task<bool> SaveChangesAsync();
    }
}