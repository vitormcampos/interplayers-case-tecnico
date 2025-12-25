using InterPlayers.Application.Exceptions;
using InterPlayers.Application.Interfaces.Repositories;
using InterPlayers.Application.Interfaces.Services;
using InterPlayers.Contracts.DTOs.Products;
using InterPlayers.Domain;

namespace InterPlayers.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;

    public ProductService(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    public async Task<Product> AddAsync(CreateProductDto dto)
    {
        var product = new Product(dto.Name, dto.Price);

        return await productRepository.CreateAsync(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await productRepository.GetById(id);

        if (product is null)
        {
            throw new NotFoundException("product", id.ToString());
        }

        await productRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var products = await productRepository.GetAllAsync();

        return products ?? [];
    }

    public async Task<Product> GetAsync(int id)
    {
        var product = await productRepository.GetById(id);

        if (product is null)
        {
            throw new NotFoundException("product", id.ToString());
        }

        return product;
    }

    public async Task<Product> UpdateAsync(int id, CreateProductDto dto)
    {
        var product = await productRepository.GetById(id);

        if (product is null)
        {
            throw new NotFoundException("product", id.ToString());
        }

        product.Update(dto.Name, dto.Price);

        return await productRepository.UpdateAsync(id, product);
    }
}
