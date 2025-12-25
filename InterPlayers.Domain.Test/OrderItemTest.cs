using InterPlayers.Domain.Exceptions;

namespace InterPlayers.Domain.Test;

public class OrderItemTest
{
    [Fact]
    public void ShouldCreateOrderItem()
    {
        // Arrange
        var productId = 1;
        var orderId = 1;
        var quantity = 1;
        var unitPrice = 10;

        // Action
        var orderItem = new OrderItem(productId, orderId, quantity, unitPrice);

        // Assert
        Assert.NotNull(orderItem);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(null)]
    public void ShouldThrowWhenProductIdInvalid(int invalidProductId)
    {
        // Arrange
        var orderId = 1;
        var quantity = 1;
        var unitPrice = 10;

        void assertAction()
        {
            // Action
            var orderItem = new OrderItem(invalidProductId, orderId, quantity, unitPrice);
        }

        // Assert
        Assert.Throws<DomainValidationException>(assertAction);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(null)]
    public void ShouldThrowWhenOrderIdInvalid(int invalidOrderId)
    {
        // Arrange
        var productId = 1;
        var quantity = 1;
        var unitPrice = 10;

        void assertAction()
        {
            // Action
            var orderItem = new OrderItem(productId, invalidOrderId, quantity, unitPrice);
        }

        // Assert
        Assert.Throws<DomainValidationException>(assertAction);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(null)]
    public void ShouldThrowWhenQuantityInvalid(int invalidQuantity)
    {
        // Arrange
        var productId = 1;
        var orderId = 1;
        var unitPrice = 10;

        void assertAction()
        {
            // Action
            var orderItem = new OrderItem(productId, orderId, invalidQuantity, unitPrice);
        }

        // Assert
        Assert.Throws<DomainValidationException>(assertAction);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-7)]
    public void ShouldThrowWhenUnitPriceIsInvalid(decimal invalidPrice)
    {
        // Arrange
        var productId = 1;
        var orderId = 1;
        var quantity = 1;

        // Action
        void assertAction()
        {
            var orderItem = new OrderItem(1, 1, 1, invalidPrice);
        }

        // Assert
        Assert.Throws<DomainValidationException>(assertAction);
    }

    [Fact]
    public void ShouldUpdateOrderItem()
    {
        // Arrange
        var productId = 1;
        var orderId = 1;
        var quantity = 1;
        var unitPrice = 10;
        var currentOrderItem = new OrderItem(productId, orderId, quantity, unitPrice);

        // Action
        var newQuantity = 2;
        currentOrderItem.Update(productId, newQuantity, unitPrice);

        // Assert
        Assert.Equal(newQuantity, currentOrderItem.Quantity);
    }
}
