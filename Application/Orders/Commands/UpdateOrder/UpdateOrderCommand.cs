namespace ApiEcommerce.Application.Orders.Commands.UpdateOrder;

using ApiEcommerce.Application.Orders.DTOs;
using MediatR;


public class UpdateOrderCommand : IRequest<bool>
{
    public int Id { get; private set; }
    public int BuyerId { get; init; }
    public List<OrderItemDto> Itens { get; init; }

    public void SetId(int id) => Id = id;
}
