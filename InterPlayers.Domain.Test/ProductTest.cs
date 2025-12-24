namespace InterPlayers.Domain.Test;

public class ProductTest
{
    [Fact]
    public void ShouldCreateProduct()
    {
        // Arrange
        var productName = "New Product 1";
        var productPrice = 10;

        // Action
        var product = new Product(productName, productPrice);

        // Assert
        Assert.NotNull(product);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldThrowsIfProductNameInvalid(string productName)
    {
        // Arrange
        var productPrice = 10;

        void actionAssert()
        {
            // Action
            var product = new Product(productName, productPrice);
        }

        // Assert
        Assert.Throws<ArgumentException>(actionAssert);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(null)]
    public void ShouldThrowsIfProductPriceInvalid(decimal productPrice)
    {
        // Arrange
        var productName = "Product 1";

        void actionAssert()
        {
            // Action
            var product = new Product(productName, productPrice);
        }

        // Assert
        Assert.Throws<ArgumentException>(actionAssert);
    }

    [Fact]
    public void ShouldUpdateProduct()
    {
        // Arrange
        var currentProduct = new Product("Product 1", 10);
        var newProductName = "Product 2";
        var newProductPrice = 15;

        // Action
        currentProduct.SetName(newProductName);
        currentProduct.SetPrice(newProductPrice);

        // Assert
        Assert.Equal(currentProduct.Name, newProductName);
        Assert.Equal(currentProduct.Price, newProductPrice);
    }
}
