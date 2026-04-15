namespace ApiEcommerce.Application.Orders.Commands.CancelOrder;

using MediatR;
using ApiEcommerce.Infrastructure.Persistence.Repositories;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _repo;

    public CancelOrderHandler(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(request.Id);
        if (order is null) return false;

        order.Cancel();
        await _repo.UpdateAsync(order);

        return true;
    }
}
