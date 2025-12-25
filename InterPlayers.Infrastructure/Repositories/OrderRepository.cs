using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Transactions;
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
        using var connection = await connectionFactory.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var orderId = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO [dbo].[Orders] (Total) 
                  VALUES (@Total);
                  SELECT CAST(SCOPE_IDENTITY() as int);",
                new { order.Total },
                transaction
            );

            transaction.Commit();

            return await GetById(orderId);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync(
        int orderId,
        int productId,
        string productName
    )
    {
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            var lookup = new Dictionary<int, Order>();

            object? _orderId = orderId > 0 ? orderId : null;
            object? _productId = productId > 0 ? productId : null;
            string? _productName = string.IsNullOrWhiteSpace(productName) ? null : productName;

            var result = await dbConnection.QueryAsync<Order, OrderItem, Product, Order>(
                "sp_GetOrders",
                (order, item, product) =>
                {
                    if (!lookup.TryGetValue(order.Id, out var ord))
                    {
                        ord = order;
                        lookup.Add(ord.Id, ord);
                    }

                    if (item != null)
                    {
                        item.Product = product;
                        ord.AddItem(item);
                    }

                    return ord;
                },
                param: new
                {
                    OrderId = _orderId,
                    ProductId = _productId,
                    ProductName = _productName,
                },
                commandType: CommandType.StoredProcedure
            );

            return lookup.Values;
        }
    }

    public async Task<Order?> GetById(int id)
    {
        using (var dbConnection = await connectionFactory.GetConnectionAsync())
        {
            return await dbConnection.QueryFirstOrDefaultAsync<Order>(
                @"SELECT * FROM [dbo].[Orders] WHERE [Id] = @Id",
                new { Id = id }
            );
        }
    }

    public async Task<Order> GetWithItemsById(int id)
    {
        using var connection = await connectionFactory.GetConnectionAsync();

        var sql =
            @"
                SELECT
                    *
                FROM Orders O
                LEFT JOIN OrderItems OI ON O.Id = OI.OrderId
                WHERE O.Id = @Id;
            ";

        Order? order = null;
        var orderItems = new Dictionary<int, Order>();

        await connection.QueryAsync<Order, OrderItem, Order>(
            sql,
            (o, item) =>
            {
                if (!orderItems.TryGetValue(o.Id, out var existingOrder))
                {
                    existingOrder = o;
                    orderItems.Add(o.Id, existingOrder);
                }

                if (item != null && item.Id != 0)
                {
                    existingOrder.AddItem(item);
                }

                return existingOrder;
            },
            new { Id = id },
            splitOn: "Id"
        );

        order = orderItems.Values.FirstOrDefault();
        return order;
    }

    public async Task Update(int id, Order order)
    {
        using var connection = await connectionFactory.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var existingItemsSql =
                @"
                    SELECT 
                        *
                    FROM OrderItems
                    WHERE OrderId = @OrderId;
                ";

            var existingItems = await connection.QueryAsync<OrderItem>(
                existingItemsSql,
                new { OrderId = id },
                transaction
            );

            var existingItemsMap = existingItems.ToDictionary(i => i.Id);

            foreach (var item in order.Items)
            {
                if (item.Id == 0)
                {
                    await InsertItem(id, item, connection, transaction);
                }
                else
                {
                    if (existingItemsMap.ContainsKey(item.Id))
                    {
                        await UpdateItem(item, connection, transaction);

                        existingItemsMap.Remove(item.Id);
                    }
                }
            }

            if (existingItemsMap.Count != 0)
            {
                var idsToDelete = existingItemsMap.Keys.ToList();
                await DeleteItem(idsToDelete, connection, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private async Task InsertItem(
        int id,
        OrderItem item,
        DbConnection connection,
        DbTransaction transaction
    )
    {
        var insertItemSql =
            @"
                INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
            ";

        await connection.ExecuteAsync(
            insertItemSql,
            new
            {
                OrderId = id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
            },
            transaction
        );
    }

    private async Task UpdateItem(
        OrderItem item,
        DbConnection connection,
        DbTransaction transaction
    )
    {
        var updateItemSql =
            @"
                UPDATE OrderItems
                SET Quantity = @Quantity,
                    UnitPrice = @UnitPrice,
                    ProductId = @ProductId
                WHERE Id = @Id;
            ";

        await connection.ExecuteAsync(
            updateItemSql,
            new
            {
                item.Id,
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
            },
            transaction
        );
    }

    private async Task DeleteItem(
        List<int> idsToDelete,
        DbConnection connection,
        DbTransaction transaction
    )
    {
        var deleteItemSql =
            @"
                DELETE FROM OrderItems
                WHERE Id IN @Ids;
            ";

        await connection.ExecuteAsync(deleteItemSql, new { Ids = idsToDelete }, transaction);
    }
}
