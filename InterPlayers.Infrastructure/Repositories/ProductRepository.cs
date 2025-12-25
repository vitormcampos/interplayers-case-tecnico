using Dapper;
using InterPlayers.Application.Interfaces.Factories;
using InterPlayers.Application.Interfaces.Repositories;
using InterPlayers.Domain;

namespace InterPlayers.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IConnectionFactory connectionFactory;

    public ProductRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        const string sql =
            @"
                INSERT INTO [dbo].[Products] (Name, Price)
                VALUES (@Name, @Price);

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            var productId = await dbConnection.ExecuteScalarAsync<int>(
                sql,
                new { Name = product.Name, Price = product.Price }
            );

            return await dbConnection.QueryFirstAsync<Product>(
                "SELECT * FROM [dbo].[Products] WHERE Id = @Id",
                new { Id = productId }
            );
        }
    }

    public async Task DeleteAsync(int id)
    {
        var sql =
            @"
            DELETE FROM [dbo].[Products]
            WHERE [Id] = @Id
        ";
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            await dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            return await dbConnection.QueryAsync<Product>("SELECT * FROM [dbo].[Products]");
        }
    }

    public async Task<Product?> GetById(int id)
    {
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            return await dbConnection.QueryFirstOrDefaultAsync<Product>(
                "SELECT * FROM Products WHERE [Id] = @Id",
                new { Id = id }
            );
        }
    }

    public async Task<Product> UpdateAsync(int id, Product product)
    {
        var sql =
            @"
                UPDATE [dbo].[Products]
                   SET [Name] = @Name,
                       [Price] = @Price
                 WHERE [Id] = @Id
            ";
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            await dbConnection.ExecuteAsync(
                sql,
                new
                {
                    Id = id,
                    Name = product.Name,
                    Price = product.Price,
                }
            );

            return await dbConnection.QueryFirstAsync<Product>(
                "SELECT * FROM Products WHERE [Id] = @Id",
                new { Id = id }
            );
        }
    }
}
