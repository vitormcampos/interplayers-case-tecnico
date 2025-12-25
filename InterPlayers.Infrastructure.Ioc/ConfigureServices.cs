using InterPlayers.Application.Interfaces.Factories;
using InterPlayers.Application.Interfaces.Repositories;
using InterPlayers.Application.Interfaces.Services;
using InterPlayers.Application.Services;
using InterPlayers.Application.Test.Interfaces.Repositories;
using InterPlayers.Infrastructure.Factories;
using InterPlayers.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InterPlayers.Infrastructure.Ioc;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder app)
    {
        var connectionString = app.Configuration.GetConnectionString(
            "InterPlayersConnectionString"
        );

        app.Services.AddScoped<IConnectionFactory>(options => new SqlServerConnectionFactory(
            connectionString
        ));
        app.Services.AddScoped<IProductRepository, ProductRepository>();
        app.Services.AddScoped<IProductService, ProductService>();
        app.Services.AddScoped<IOrderRepository, OrderRepository>();
        app.Services.AddScoped<IOrderService, OrderService>();
    }
}
