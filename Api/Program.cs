using MediatR;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiEcommerce.Application.Behaviors;
using ApiEcommerce.Infrastructure.Persistence;
using ApiEcommerce.Infrastructure.Persistence.Repositories;
using ApiEcommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ApiEcommerce.Application;
using Microsoft.OpenApi.Models;
using ApiEcommerce.Application.Orders.Commands.Login;
using ApiEcommerce.Application.Orders.Commands.CreateOrder;
using ApiEcommerce.Application.Orders.Commands.UpdateOrder;
using ApiEcommerce.Api.Config;

var builder = WebApplication.CreateBuilder(args);

#region Serilog
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();
#endregion

#region DB
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
#endregion

#region DI
builder.Services.AddApplicationServices(builder.Configuration);
#endregion

#region Redis
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration.GetConnectionString("Redis"));
#endregion

#region JWT
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
#endregion

var app = builder.Build();

#region Middleware ProblemDetails
app.UseExceptionHandler(a => a.Run(async ctx => {
    ctx.Response.StatusCode = 500; ctx.Response.ContentType = "application/problem+json";
    await ctx.Response.WriteAsJsonAsync(new { type = "https://httpstatuses.com/500", title = "Erro interno", status = 500 });
}));
#endregion

#region Uses e Add
app.UseSerilogRequestLogging();
app.UseAuthentication(); 
app.UseAuthorization();
app.UseSwagger(); 
app.UseSwaggerUI();
#endregion

#region Swagger
var auth = app.MapGroup("/api/v1/auth").AllowAnonymous();

auth.MapPost("/login", async (LoginRequest request, IMediator mediator) =>
{
    var token = await mediator.Send(request);

    if (token is null)
        return Results.Unauthorized();

    return Results.Ok(new { token });
}).AllowAnonymous().WithTags("Auth");

var g = app.MapGroup("/api/v1/orders").RequireAuthorization();

#region TODO: MELHORIAS FUTURAS, ACREDITO QUE DÁ PARA ENXUGAR
g.MapPost("/", async (CreateOrderCommand c, IMediator m) => Results.Created($"/api/v1/orders/{await m.Send(c)}", null));

g.MapGet("/", async (OrderStatus? s, IMediator m) => Results.Ok(await m.Send(new ApiEcommerce.Application.Orders.Queries.GetOrders.GetOrdersQuery(s))));

g.MapGet("/{id}", async (int id, IMediator m) =>
{
    var o = await m.Send(new ApiEcommerce.Application.Orders.Queries.GetOrdersById.GetOrdersByIdQuery(id));
    return o is null ? Results.NotFound() : Results.Ok(o);
});

g.MapPut("/{id}", async (int id, UpdateOrderCommand cmd, IMediator m) =>
{
    cmd.SetId(id);

    var result = await m.Send(cmd);

    return result ? Results.Ok() : Results.NotFound();
});

g.MapPatch("/{id}/cancel", async (int id, IMediator m) => (await m.Send(new ApiEcommerce.Application.Orders.Commands.CancelOrder.CancelOrderCommand(id)) ? Results.Ok() : Results.NotFound()));

g.MapDelete("/{id}", async (int id, IMediator m) => (await m.Send(new ApiEcommerce.Application.Orders.Commands.DeleteOrder.DeleteOrderCommand(id)) ? Results.Ok() : Results.NotFound()));

app.MapHealthChecks("/health");

#endregion

#endregion

app.Run();