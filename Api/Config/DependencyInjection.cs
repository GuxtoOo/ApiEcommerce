using ApiEcommerce.Application.Behaviors;
using ApiEcommerce.Infrastructure.Persistence.Repositories;
using ApiEcommerce.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ApiEcommerce.Application;

namespace ApiEcommerce.Api.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // Database
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        // Repository
        services.AddScoped<IOrderRepository, OrderRepository>();

        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly)
        );

        // Pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>));

        // Redis
        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = config.GetConnectionString("Redis");
        });

        return services;
    }
}
