namespace Orders.Api.Entities;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<Order> Orders { get; set; } = [];
}
