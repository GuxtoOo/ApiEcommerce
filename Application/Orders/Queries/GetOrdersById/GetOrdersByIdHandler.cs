namespace ApiEcommerce.Application.Orders.Queries.GetOrdersById;

using MediatR;
using Infrastructure.Persistence.Repositories;
using ApiEcommerce.Domain.Entities;

public class GetOrderByIdHandler : IRequestHandler<GetOrdersByIdQuery, Order?>
{
    private readonly IOrderRepository _repo;

    public GetOrderByIdHandler(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<Order?> Handle(GetOrdersByIdQuery request, CancellationToken ct)
    {
        return await _repo.GetByIdAsync(request.Id);
    }
}
