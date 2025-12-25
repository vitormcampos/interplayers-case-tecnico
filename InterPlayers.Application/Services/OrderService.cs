using InterPlayers.Application.Exceptions;
using InterPlayers.Application.Interfaces.Services;
using InterPlayers.Application.Test.Interfaces.Repositories;
using InterPlayers.Contracts.DTOs.Orders;
using InterPlayers.Domain;

namespace InterPlayers.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        this.orderRepository = orderRepository;
    }

    public async Task<Order> AddAsync(CreateOrderDto dto)
    {
        var order = new Order();

        return await orderRepository.CreateAsync(order);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(
        int orderId,
        int productId,
        string productName
    )
    {
        var orders = await orderRepository.GetAllAsync(orderId, productId, productName);

        return orders ?? [];
    }

    public async Task<Order> GetAsync(int id)
    {
        var order = await orderRepository.GetWithItemsById(id);

        if (order is null)
        {
            throw new NotFoundException("order", id.ToString());
        }

        return order;
    }

    public async Task<Order> Update(int id, Order order)
    {
        var orderExist = await orderRepository.GetById(id);

        if (orderExist is null)
        {
            throw new NotFoundException("order", id.ToString());
        }

        await orderRepository.Update(id, order);

        return await orderRepository.GetWithItemsById(id);
    }
}
