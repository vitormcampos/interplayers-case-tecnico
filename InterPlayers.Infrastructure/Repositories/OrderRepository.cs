using Dapper;
using InterPlayers.Application.Interfaces.Factories;
using InterPlayers.Application.Test.Interfaces.Repositories;
using InterPlayers.Domain;

namespace InterPlayers.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IConnectionFactory connectionFactory;

    public OrderRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        const string sql =
            @"
                INSERT INTO [dbo].[Orders] (Total) Values (0);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            var orderId = await dbConnection.ExecuteScalarAsync<int>(sql);

            return await dbConnection.QueryFirstAsync<Order>(
                "SELECT * FROM [dbo].[Orders] WHERE Id = @Id",
                new { Id = orderId }
            );
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            return await dbConnection.QueryAsync<Order>("SELECT * FROM [dbo].[Orders]");
        }
    }

    public async Task<Order?> GetById(int id)
    {
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            return await dbConnection.QueryFirstOrDefaultAsync<Order>(
                "SELECT * FROM [dbo].[Orders] WHERE [Id] = @Id",
                new { Id = id }
            );
        }
    }
}
