namespace ApiEcommerce.Application.Orders.Commands.Login;

using MediatR;

public record LoginRequest(string Email, string Password) : IRequest<string>;