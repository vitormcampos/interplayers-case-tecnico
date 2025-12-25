using InterPlayers.Contracts.DTOs.Products;
using InterPlayers.Domain;

namespace InterPlayers.Application.Interfaces.Services;

public interface IProductService
{
    Task<Product> AddAsync(CreateProductDto dto);
    Task<Product> UpdateAsync(int id, CreateProductDto dto);
    Task DeleteAsync(int id);
    Task<Product> GetAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
}
