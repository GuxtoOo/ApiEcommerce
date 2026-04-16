using MediatR;
using Serilog;
using ApiEcommerce.Infrastructure.Persistence;
using ApiEcommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ApiEcommerce.Application.Orders.Commands.Login;
using ApiEcommerce.Application.Orders.Commands.CreateOrder;
using ApiEcommerce.Application.Orders.Commands.UpdateOrder;
using ApiEcommerce.Api.Config;

var builder = WebApplication.CreateBuilder(args);

#region Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
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
    await ctx.Response.WriteAsJsonAsync(new
    {
        type = "https://httpstatuses.com/500",
        title = "Erro interno no servidor",
        status = 500,
        detail = "Ocorreu um erro inesperado",
        instance = ctx.Request.Path
    });
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

#region TODO: MELHORIAS FUTURAS, ACREDITO QUE DÁ PARA ENXUGAR
var auth = app.MapGroup("/api/v1/auth").AllowAnonymous();

auth.MapPost("/login", async (LoginRequest request, IMediator mediator) =>
{
    var token = await mediator.Send(request);

    if (token is null)
        return Results.Unauthorized();

    return Results.Ok(new { token });
}).AllowAnonymous().WithTags("Auth");

var g = app.MapGroup("/api/v1/orders").RequireAuthorization();

g.MapPost("/", async (CreateOrderCommand c, IMediator m) => Results.Created($"/api/v1/orders/{await m.Send(c)}", null)).WithTags("Orders").WithName("PostOrder");

g.MapGet("/", async (OrderStatus? s, IMediator m) => Results.Ok(await m.Send(new ApiEcommerce.Application.Orders.Queries.GetOrders.GetOrdersQuery(s)))).WithTags("Orders").WithName("GetOrders");

g.MapGet("/{id}", async (int id, IMediator m) =>
{
    var o = await m.Send(new ApiEcommerce.Application.Orders.Queries.GetOrdersById.GetOrdersByIdQuery(id));
    return o is null ? Results.NotFound() : Results.Ok(o);
}).WithTags("Orders").WithName("GetOrderById");

g.MapPut("/{id}", async (int id, UpdateOrderCommand cmd, IMediator m) =>
{
    cmd.SetId(id);

    var result = await m.Send(cmd);

    return result ? Results.Ok() : Results.NotFound();
}).WithTags("Orders").WithName("PutOrder");

g.MapPatch("/{id}/cancel", async (int id, IMediator m) => await m.Send(new ApiEcommerce.Application.Orders.Commands.CancelOrder.CancelOrderCommand(id)) ? Results.Ok() : Results.NotFound()).WithTags("Orders").WithName("CancelOrder");

g.MapDelete("/{id}", async (int id, IMediator m) => await m.Send(new ApiEcommerce.Application.Orders.Commands.DeleteOrder.DeleteOrderCommand(id)) ? Results.Ok() : Results.NotFound()).WithTags("Orders").WithName("RemoveOrder");

app.MapHealthChecks("/health");

#endregion

#endregion

app.Run();