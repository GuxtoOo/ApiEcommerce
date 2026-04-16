namespace ApiEcommerce.Application.Orders.Commands.Login;

using ApiEcommerce.Infrastructure.Interfaces;
using MediatR;

public class LoginHandler : IRequestHandler<LoginRequest, string>
{
    private readonly IJwtService _jwtService;

    public LoginHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<string> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        // Simulação de validação
        if (request.Email != "admin@email.com")
            return null;

        return _jwtService.GenerateToken(1, request.Email);
    }
}