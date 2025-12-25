using System.Data.Common;

namespace InterPlayers.Application.Interfaces.Factories;

public interface IConnectionFactory
{
    Task<DbConnection> GetConnectionAsync();
}
