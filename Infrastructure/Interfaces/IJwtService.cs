namespace ApiEcommerce.Infrastructure.Interfaces;

public interface IJwtService
{
    string GenerateToken(int userId, string email);
}
