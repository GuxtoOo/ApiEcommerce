using MediatR;

namespace ApiEcommerce.Application.Orders.Commands.DeleteOrder
{
    public record DeleteOrderCommand(int Id) : IRequest<bool>;
}
