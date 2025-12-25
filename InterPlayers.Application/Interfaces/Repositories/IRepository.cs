namespace InterPlayers.Application.Interfaces.Repositories;

public interface IRepository<T>
    where T : class
{
    Task<T> GetById(int productId);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(int id, T entity);
    Task DeleteAsync(int id);
}
