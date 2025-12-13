using BeatVault.API.Entities;

namespace BeatVault.API.Interfaces
{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
    }
}