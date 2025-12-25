using InterPlayers.Contracts.DTOs.Orders;
using InterPlayers.Domain;

namespace InterPlayers.Application.Interfaces.Services;

public interface IOrderService
{
    Task<Order> AddAsync(CreateOrderDto dto);
    Task<Order> GetAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync(int orderId, int productId, string productName);
    Task<Order> Update(int orderId, Order order);
}
