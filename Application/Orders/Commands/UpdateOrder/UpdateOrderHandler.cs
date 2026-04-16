namespace ApiEcommerce.Application.Orders.Commands.UpdateOrder;

using ApiEcommerce.Domain.Entities;
using ApiEcommerce.Infrastructure.Persistence.Repositories;
using MediatR;

public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, bool>
{
    private readonly IOrderRepository _repo;

    public UpdateOrderHandler(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(request.Id);

        if (order is null)
            return false;

        var itens = request.Itens
            .Select(i => new OrderItems(i.ProductId, i.Price, i.Quantity))
            .ToList();

        order.Update(itens);

        await _repo.UpdateAsync(order);

        return true;
    }
}
