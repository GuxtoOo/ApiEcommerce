namespace ApiEcommerce.Application.Orders.DTOs;

public record OrderItemDto(int ProductId, decimal Price, int Quantity);
