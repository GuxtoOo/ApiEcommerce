namespace ApiEcommerce.Application.Orders.Queries.GetOrders;

using MediatR;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using ApiEcommerce.Application.Orders.DTOs;
using ApiEcommerce.Infrastructure.Persistence;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly AppDbContext _context;

    public GetOrdersHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var query = _context.Orders.AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        return await query
            .Select(o => new OrderDto(
                o.Id,
                o.BuyerId,
                o.Status
            ))
            .ToListAsync(ct);
    }
}
