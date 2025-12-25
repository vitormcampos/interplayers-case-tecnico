using System.Data.Common;
using InterPlayers.Application.Interfaces.Factories;
using Microsoft.Data.SqlClient;

namespace InterPlayers.Infrastructure.Factories;

public class SqlServerConnectionFactory : IConnectionFactory
{
    private readonly string connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<DbConnection> GetConnectionAsync()
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
