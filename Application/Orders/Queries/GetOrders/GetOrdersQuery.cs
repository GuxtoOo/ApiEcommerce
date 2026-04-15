namespace ApiEcommerce.Application.Orders.Queries.GetOrders;

using MediatR;
using Domain.Enums;
using Domain.Entities;
using Application.Common;

public record GetOrdersQuery(OrderStatus? Status)
    : IRequest<List<Order>>, ICacheable
{
    public string CacheKey => $"orders_{Status}";
    public int ExpirationMinutes => 5;
}
