namespace Orders.Api.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Cancelled
}
