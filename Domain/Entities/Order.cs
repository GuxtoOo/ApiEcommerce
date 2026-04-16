using ApiEcommerce.Domain.Enums;

namespace ApiEcommerce.Domain.Entities;

public class OrderItems
{
    public int ProductId { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice { get; private set; }
    private OrderItems() { }

    public OrderItems(int productId, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade dos itens informada deve ser maior que zero.");

        ProductId = productId;
        Price = price;
        Quantity = quantity;
        TotalPrice = price * quantity;
    }
}

public class Order
{
    public int Id { get; private set; }
    public int BuyerId { get; private set; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItems> _itens = new();
    public IReadOnlyCollection<OrderItems> itens => _itens;

    private Order() { }

    public Order(int buyerId)
    {
        BuyerId = buyerId;
        Status = OrderStatus.Iniciado;
    }

    public static Order Create(int buyerId, List<OrderItems> itens)
    {
        if (itens == null || !itens.Any())
            throw new ArgumentException("O pedido deve possuir ao menos um item.");

        var order = new Order(buyerId);

        foreach (var item in itens)
            order._itens.Add(item);

        return order;
    }

    public void Update(List<OrderItems> itens)
    {
        if (Status != OrderStatus.Iniciado)
            throw new InvalidOperationException("Não é possível alterar informações de pedidos com status diferente de 'Iniciado'.");

        _itens.Clear();
        _itens.AddRange(itens);
    }

    public void Cancel()
    {
        if (Status is not (OrderStatus.Iniciado or OrderStatus.Processado))
            throw new InvalidOperationException("Não é possível realizar o cancelamento de pedidos com status 'Enviado' ou 'Cancelado'.");

        Status = OrderStatus.Cancelado;
    }
}
