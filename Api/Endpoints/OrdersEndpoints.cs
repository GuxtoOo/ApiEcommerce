using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ApiEcommerce.Domain.Enums;

namespace ApiEcommerce.Api.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/orders")
                       .RequireAuthorization();

        group.MapPost("/", async (Command cmd, IMediator mediator) =>
        {
            var id = await mediator.Send(cmd);
            return Results.Created($"/api/v1/orders/{id}", id);
        });

        group.MapGet("/", async (OrderStatus? status, IMediator mediator) =>
        {
            var result = await mediator.Send(new Application.Orders.Queries.GetOrders.GetOrdersQuery(status));
            return Results.Ok(result);
        });

        group.MapGet("/{id}", async (int id, IMediator mediator) =>
        {
            var order = await mediator.Send(new Application.Orders.Queries.GetOrdersById.GetOrdersByIdQuery(id));
            return order is null ? Results.NotFound() : Results.Ok(order);
        });

        group.MapPut("/{id}", async (int id, Application.Orders.Commands.UpdateOrder.UpdateOrderCommand cmd, IMediator mediator) =>
        {
            if (id != cmd.Id) return Results.BadRequest();

            var updated = await mediator.Send(cmd);
            return updated ? Results.Ok() : Results.NotFound();
        });

        group.MapPatch("/{id}/cancel", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new Application.Orders.Commands.CancelOrder.CancelOrderCommand(id));
            return result ? Results.Ok() : Results.NotFound();
        });

        group.MapDelete("/{id}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new Application.Orders.Commands.DeleteOrder.DeleteOrderCommand(id));
            return result ? Results.Ok() : Results.NotFound();
        });
    }
}
