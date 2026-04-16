namespace ApiEcommerce.Application.Orders.Commands.CreateOrder;

using MediatR;
using Application.Orders.DTOs;

public record CreateOrderCommand(
    int BuyerId,
    List<OrderItemDto> itens
) : IRequest<int>;

public record CreateOrderItemDto(
    int ProductId,
    decimal Price,
    int Quantity,
    decimal TotalPrice
);