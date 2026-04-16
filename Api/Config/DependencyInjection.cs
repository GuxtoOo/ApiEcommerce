using ApiEcommerce.Application.Behaviors;
using ApiEcommerce.Infrastructure.Persistence.Repositories;
using ApiEcommerce.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ApiEcommerce.Application;
using ApiEcommerce.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiEcommerce.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

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
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>));

        // Redis
        //services.AddStackExchangeRedisCache(opt =>
        //{
        //    opt.Configuration = config.GetConnectionString("Redis");
        //});

        //JWT
        services.AddScoped<IJwtService, JwtService>();

        var jwt = config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwt["SecretKey"]);
        if (string.IsNullOrEmpty(jwt["SecretKey"]))
            throw new Exception("JWT SecretKey não configurada");

        services.AddAuthentication(op => { 
        op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        services.AddAuthorization();

        return services;
    }
}
