namespace ApiEcommerce.Application.Orders.Commands.CancelOrder;

using MediatR;

public record CancelOrderCommand(int Id) : IRequest<bool>;
