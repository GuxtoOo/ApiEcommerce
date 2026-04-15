namespace ApiEcommerce.Application.Orders.Commands.UpdateOrder;

using ApiEcommerce.Application.Orders.DTOs;
using MediatR;

public record UpdateOrderCommand(
    int Id,
    List<OrderItemDto> Items
) : IRequest<bool>;
