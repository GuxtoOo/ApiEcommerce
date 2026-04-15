using ApiEcommerce.Domain.Enums;

namespace ApiEcommerce.Domain.Entities;

public class OrderItem
{
    public int ProductId { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public OrderItem(int productId, decimal price, int quantity)
    { ProductId = productId; Price = price; Quantity = quantity; }
}

public class Order
{
    public int Id { get; private set; }
    public int BuyerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    public Order(int buyerId, List<OrderItem> items)
    {
        if (!items.Any()) throw new ArgumentException("O pedido deve possuir ao menos um item.");
        BuyerId = buyerId; Status = OrderStatus.Iniciado; Items = items;
    }

    public void Update(List<OrderItem> items)
    {
        if (Status != OrderStatus.Iniciado) throw new InvalidOperationException("Não é possível alterar informações de pedidos com status diferente de 'Iniciado'.");
        Items = items;
    }
    public void Cancel()
    {
        if (Status is not (OrderStatus.Iniciado or OrderStatus.Processado))
            throw new InvalidOperationException("Não é possível realizar o cancelamento de pedidos com status 'Enviado' ou 'Cancelado'.");
        Status = OrderStatus.Cancelado;
    }
}
