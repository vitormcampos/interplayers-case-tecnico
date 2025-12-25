using InterPlayers.Domain;

namespace InterPlayers.Application.Test.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order> GetById(int id);
    Task<Order> GetWithItemsById(int id);
    Task<IEnumerable<Order>> GetAllAsync(int orderId, int productId, string productName);
    Task<Order> CreateAsync(Order entity);
    Task Update(int id, Order order);
}
