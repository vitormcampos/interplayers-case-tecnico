namespace InterPlayers.Application.Test;

using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using InterPlayers.Application.Exceptions;
using InterPlayers.Application.Services;
using InterPlayers.Application.Test.Interfaces.Repositories;
using InterPlayers.Contracts.DTOs.Orders;
using InterPlayers.Domain;
using Moq;
using Xunit;

public class OrderServiceTest
{
    private readonly Faker faker = new();
    private readonly Mock<IOrderRepository> orderRepositoryMock;
    private readonly OrderService orderService;

    public OrderServiceTest()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        orderService = new OrderService(orderRepositoryMock.Object);
    }

    [Fact]
    public async Task ShouldCreateOrder()
    {
        // Arrange
        var dto = new CreateOrderDto();

        var created = new Order();

        orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync(created);

        // Action
        var result = await orderService.AddAsync(dto);

        // Assert
        Assert.NotNull(result);
        orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnAllOrders()
    {
        // Arrange
        var list = new List<Order> { new Order(), new Order() };

        orderRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

        // Action
        var result = await orderService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task ShouldReturnEmptyListIfRepositoryReturnsNull()
    {
        // Arrange
        orderRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync((IEnumerable<Order>?)null);

        // Action
        var result = await orderService.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ShouldReturnOrderById()
    {
        // Arrange
        var order = new Order();

        orderRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(order);

        // Action
        var result = await orderService.GetAsync(1);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ShouldThrowIfOrderNotFound()
    {
        // Arrange
        orderRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((Order?)null);

        async Task actionAssert()
        {
            // Action
            await orderService.GetAsync(99);
        }

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(actionAssert);
    }
}
