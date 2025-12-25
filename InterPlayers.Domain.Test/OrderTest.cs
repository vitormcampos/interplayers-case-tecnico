using InterPlayers.Domain.Exceptions;
using InterPlayers.Domain.Test.Helpers;

namespace InterPlayers.Domain.Test;

public class OrderTests
{
    private const int orderId = 10;

    private OrderItem CreateItem(int productId, decimal price, int quantity = 1)
    {
        return new OrderItem(orderId, productId, quantity, price);
    }

    [Fact]
    public void ShouldAddItemWhenValid()
    {
        // Arrange
        var order = new Order();
        var productId = 1;
        var price = 10m;
        var quantity = 2;
        var item = CreateItem(productId, price, quantity);

        // Action
        order.AddItem(item);

        // Assert
        Assert.Single(order.Items);
        Assert.Equal(price * quantity, order.Total);
    }

    [Fact]
    public void ShouldThrowWhenItemIsNull()
    {
        // Arrange
        var order = new Order();

        // Action
        void assertAction()
        {
            order.AddItem(null);
        }

        // Assert
        Assert.Throws<DomainValidationException>(assertAction);
    }

    [Fact]
    public void ShouldSumQuantityWhenDuplicateProduct()
    {
        // Arrange
        var order = new Order();
        var productId = 1;
        var price = 10m;
        var quantity = 2;
        order.AddItem(CreateItem(productId, price, quantity));

        var duplicate = CreateItem(productId, price, quantity);
        order.AddItem(duplicate);

        // Assert
        Assert.Equal(order.Items.FirstOrDefault().Quantity, 4);
    }

    [Fact]
    public void ShouldRemoveWhenExists()
    {
        // Arrange
        var order = new Order();
        var productId1 = 1;
        var productId2 = 2;
        var price = 10m;
        var quantity = 2;
        var item1 = CreateItem(productId1, price, quantity);
        var item2 = CreateItem(productId2, price, quantity);
        SetIDHelper.SetId(item1, 1);
        SetIDHelper.SetId(item2, 2);
        order.AddItem(item1);
        order.AddItem(item2);

        // Action
        order.RemoveItem(1);

        // Assert
        Assert.Single(order.Items);
        Assert.Equal(price * quantity, order.Total);
    }

    [Fact]
    public void ShouldThrowWhenNotFound()
    {
        // Arrange
        var order = new Order();

        // Action
        void assertAction()
        {
            order.RemoveItem(99);
        }

        // Assert
        Assert.Throws<DomainValidationException>(assertAction);
    }

    [Fact]
    public void ShouldUpdateWhenValid()
    {
        // Arrange
        var order = new Order();
        var itemId = 1;
        var productId = 1;
        var item = CreateItem(productId, 10m, 1);
        SetIDHelper.SetId(item, itemId);
        order.AddItem(item);

        // Action
        order.UpdateItem(itemId, productId, 3, 1);

        // Assert
        var updated = order.Items.First(i => i.ProductId == productId);
        Assert.Equal(3, updated.Quantity);
        Assert.Equal(3, order.Total);
    }

    [Fact]
    public void ShouldThrowWhenItemNotFound()
    {
        // Arrange
        var order = new Order();
        var itemId = 1;
        var productId = 1;
        var item = CreateItem(1, 10, 1);
        SetIDHelper.SetId(item, itemId);

        // Action
        void assertAction()
        {
            order.UpdateItem(itemId, productId, 1, 5);
        }

        //Assert
        Assert.Throws<DomainValidationException>(assertAction);
    }

    [Fact]
    public void ShouldBeZeroWhenNoItems()
    {
        // Arrange
        var order = new Order();

        // Assert
        Assert.Equal(0m, order.Total);
    }

    [Fact]
    public void ShouldRecalculateAfterMultipleOperations()
    {
        // Arrange
        var order = new Order();
        var item1Id = 1;
        var item2Id = 2;
        var productId1 = 1;
        var productId2 = 2;
        var item1 = CreateItem(productId1, 10m, 1);
        var item2 = CreateItem(productId2, 5m, 2);
        SetIDHelper.SetId(item1, item1Id);
        SetIDHelper.SetId(item2, item2Id);

        // Action
        order.AddItem(item1);
        order.AddItem(item2);
        order.UpdateItem(item1Id, productId1, 3, 10);
        order.RemoveItem(item2Id);

        // Assert
        Assert.Equal(30m, order.Total);
    }
}
