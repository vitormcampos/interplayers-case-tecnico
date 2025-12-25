namespace InterPlayers.Application.Test;

using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using InterPlayers.Application.Exceptions;
using InterPlayers.Application.Interfaces.Repositories;
using InterPlayers.Application.Services;
using InterPlayers.Contracts.DTOs.Products;
using InterPlayers.Domain;
using Moq;
using Xunit;

public class ProductServiceTest
{
    private readonly Faker faker = new();

    private readonly Mock<IProductRepository> productRepositoryMock;
    private readonly ProductService productService;

    public ProductServiceTest()
    {
        productRepositoryMock = new Mock<IProductRepository>();
        productService = new ProductService(productRepositoryMock.Object);
    }

    [Fact]
    public async Task ShouldCreateProduct()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(1, 200),
        };

        var created = new Product(dto.Name, dto.Price);

        productRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(created);

        // Action
        var result = await productService.AddAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Price, result.Price);
    }

    [Fact]
    public async Task ShouldReturnAllProducts()
    {
        // Arrange
        var list = new List<Product>
        {
            new(faker.Commerce.ProductName(), faker.Random.Decimal(1, 100)),
            new(faker.Commerce.ProductName(), faker.Random.Decimal(1, 100)),
        };

        productRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

        // Action
        var result = await productService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task ShouldReturnEmptyListWhenRepositoryReturnsNull()
    {
        // Arrange
        productRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync((IEnumerable<Product>?)null);

        // Action
        var result = await productService.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ShouldReturnProductById()
    {
        // Arrange
        var product = new Product(faker.Commerce.ProductName(), faker.Random.Decimal(1, 200));

        productRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(product);

        // Action
        var result = await productService.GetAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact]
    public async Task ShouldThrowIfProductNotFound()
    {
        // Arrange
        productRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((Product?)null);

        async Task actionAssert()
        {
            // Action
            await productService.GetAsync(10);
        }

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(actionAssert);
    }

    [Fact]
    public async Task ShouldDeleteProduct()
    {
        // Arrange
        var product = new Product(faker.Commerce.ProductName(), faker.Random.Decimal(1, 200));

        productRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(product);

        productRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Action
        await productService.DeleteAsync(10);

        // Assert
        productRepositoryMock.Verify(r => r.DeleteAsync(10), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowWhenDeletingNonExistingProduct()
    {
        // Arrange
        productRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((Product?)null);

        async Task actionAssert()
        {
            // Action
            await productService.DeleteAsync(99);
        }

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(actionAssert);
    }

    [Fact]
    public async Task ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product("Old Name", 10);

        var dto = new CreateProductDto
        {
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(1, 150),
        };

        productRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(product);

        productRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Product>()))
            .ReturnsAsync((int id, Product p) => p);

        // Action
        var result = await productService.UpdateAsync(10, dto);

        // Assert
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Price, result.Price);
    }

    [Fact]
    public async Task ShouldThrowIfUpdatingNonExistingProduct()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(1, 150),
        };

        productRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((Product?)null);

        async Task actionAssert()
        {
            // Action
            await productService.UpdateAsync(123, dto);
        }

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(actionAssert);
    }
}
