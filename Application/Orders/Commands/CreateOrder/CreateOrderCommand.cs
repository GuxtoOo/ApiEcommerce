namespace ApiEcommerce.Application.Orders.Commands.CreateOrder;

using MediatR;
using Application.Orders.DTOs;

public record CreateOrderCommand(
    int BuyerId,
    List<OrderItemDto> Items
) : IRequest<int>;