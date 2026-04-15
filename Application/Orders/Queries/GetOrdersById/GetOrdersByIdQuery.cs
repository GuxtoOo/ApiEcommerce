namespace ApiEcommerce.Application.Orders.Queries.GetOrdersById;

using MediatR;
using Domain.Entities;
using Application.Common;

public record GetOrdersByIdQuery(int Id)
    : IRequest<Order?>, ICacheable
{
    public string CacheKey => $"order_{Id}";
    public int ExpirationMinutes => 5;
}
