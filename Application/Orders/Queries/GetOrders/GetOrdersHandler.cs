namespace ApiEcommerce.Application.Orders.Queries.GetOrders;

using MediatR;
using Infrastructure.Persistence.Repositories;
using ApiEcommerce.Domain.Entities;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, List<Order>>
{
    private readonly IOrderRepository _repo;

    public GetOrdersHandler(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Order>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        return await _repo.GetAllAsync(request.Status);
    }
}
