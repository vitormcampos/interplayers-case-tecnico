using InterPlayers.Domain;

namespace InterPlayers.Application.Test.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order> GetById(int productId);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> CreateAsync(Order entity);
}
