using ApiEcommerce.Domain.Entities;
using ApiEcommerce.Domain.Enums;

namespace ApiEcommerce.Application.Orders.DTOs;

public record OrderDto(int Id, int BuyerId, OrderStatus Status, IReadOnlyCollection<OrderItems> items);
