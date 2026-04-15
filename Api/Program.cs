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

var builder = WebApplication.CreateBuilder(args);

#region Serilog
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();
#endregion

#region DB
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
#endregion

#region DI
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly)
);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>));
#endregion

#region Redis
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration.GetConnectionString("Redis"));
#endregion

#region JWT
var key = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o => {
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});
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
var g = app.MapGroup("/api/v1/orders").RequireAuthorization();

#region MELHORIAS FUTURAS
g.MapPost("/", async (ApiEcommerce.Application.Orders.Commands.CreateOrder.CreateOrderCommand c, IMediator m) => Results.Created($"/api/v1/orders/{await m.Send(c)}", null));

g.MapGet("/", async (OrderStatus? s, IMediator m) => Results.Ok(await m.Send(new ApiEcommerce.Application.Orders.Queries.GetOrders.GetOrdersQuery(s))));

g.MapGet("/{id}", async (int id, IMediator m) =>
{
    var o = await m.Send(new ApiEcommerce.Application.Orders.Queries.GetOrdersById.GetOrdersByIdQuery(id));
    return o is null ? Results.NotFound() : Results.Ok(o);
});

g.MapPut("/{id}", async (int id, ApiEcommerce.Application.Orders.Commands.UpdateOrder.UpdateOrderCommand c, IMediator m) => id != c.Id ? Results.BadRequest() : (await m.Send(c) ? Results.Ok() : Results.NotFound()));

g.MapPatch("/{id}/cancel", async (int id, IMediator m) => (await m.Send(new ApiEcommerce.Application.Orders.Commands.CancelOrder.CancelOrderCommand(id)) ? Results.Ok() : Results.NotFound()));

g.MapDelete("/{id}", async (int id, IMediator m) => (await m.Send(new ApiEcommerce.Application.Orders.Commands.DeleteOrder.DeleteOrderCommand(id)) ? Results.Ok() : Results.NotFound()));

app.MapHealthChecks("/health");

#endregion

#endregion

app.Run();