using ApiEcommerce.Domain.Entities;
using ApiEcommerce.Infrastructure.Persistence.Repositories;
using MediatR;

namespace ApiEcommerce.Application.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _repo;

    public CreateOrderHandler(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var itens = request.itens
            .Select(i => new OrderItems(i.ProductId, i.Price, i.Quantity))
            .ToList();

        var order = Order.Create(request.BuyerId, itens);

        await _repo.AddAsync(order);

        return order.Id;
    }
}
