namespace ApiEcommerce.Application.Orders.Queries.GetOrders;

using MediatR;
using Domain.Enums;
using Application.Common;
using ApiEcommerce.Application.Orders.DTOs;

public record GetOrdersQuery(OrderStatus? Status)
    : IRequest<List<OrderDto>>, ICacheable
{
    public string CacheKey => Status.HasValue
    ? $"orders_status_{Status}"
    : "orders_all";
    public int ExpirationMinutes => 5;
}
