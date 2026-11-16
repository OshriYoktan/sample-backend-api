using Microsoft.EntityFrameworkCore;
using Orders.Api.Entities;

namespace Orders.Api.Data;

internal static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Dana Levi", Email = "dana@example.com", CreatedAtUtc = seedDate },
            new Customer { Id = 2, Name = "Avi Cohen", Email = "avi@example.com", CreatedAtUtc = seedDate });

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Mechanical Keyboard", Price = 120.00m, StockQuantity = 25 },
            new Product { Id = 2, Name = "Wireless Mouse", Price = 45.50m, StockQuantity = 60 },
            new Product { Id = 3, Name = "27-inch Monitor", Price = 310.00m, StockQuantity = 8 });

        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, CustomerId = 1, CreatedAtUtc = seedDate.AddDays(3), Status = OrderStatus.Shipped, TotalAmount = 165.50m },
            new Order { Id = 2, CustomerId = 1, CreatedAtUtc = seedDate.AddDays(20), Status = OrderStatus.Paid, TotalAmount = 310.00m },
            new Order { Id = 3, CustomerId = 2, CreatedAtUtc = seedDate.AddDays(10), Status = OrderStatus.Pending, TotalAmount = 91.00m });

        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 120.00m },
            new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 1, UnitPrice = 45.50m },
            new OrderItem { Id = 3, OrderId = 2, ProductId = 3, Quantity = 1, UnitPrice = 310.00m },
            new OrderItem { Id = 4, OrderId = 3, ProductId = 2, Quantity = 2, UnitPrice = 45.50m });
    }
}
