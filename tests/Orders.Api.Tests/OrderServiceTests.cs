using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Dtos;
using Orders.Api.Repositories;
using Orders.Api.Services;

namespace Orders.Api.Tests;

public class OrderServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 2, 1, 12, 0, 0, TimeSpan.Zero);

    private static (OrderService Service, OrdersDbContext Db) CreateService()
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new OrdersDbContext(options);
        db.Database.EnsureCreated(); // applies HasData seed

        var service = new OrderService(
            new OrderRepository(db),
            new CustomerRepository(db),
            new ProductRepository(db),
            new FixedTimeProvider(Now));
        return (service, db);
    }

    [Fact]
    public async Task GetCustomerOrders_ReturnsNewestFirst()
    {
        var (service, _) = CreateService();

        var orders = await service.GetCustomerOrdersAsync(1, CancellationToken.None);

        Assert.NotNull(orders);
        Assert.Equal([2, 1], orders.Select(o => o.Id));
    }

    [Fact]
    public async Task GetCustomerOrders_UnknownCustomer_ReturnsNull()
    {
        var (service, _) = CreateService();

        Assert.Null(await service.GetCustomerOrdersAsync(999, CancellationToken.None));
    }

    [Fact]
    public async Task CreateOrder_ComputesTotalAndDecrementsStock()
    {
        var (service, db) = CreateService();
        var request = new CreateOrderRequest(2, [new(1, 2), new(2, 1)]);

        var order = await service.CreateOrderAsync(request, CancellationToken.None);

        Assert.Equal(2 * 120.00m + 45.50m, order.TotalAmount);
        Assert.Equal("Pending", order.Status);
        Assert.Equal(Now.UtcDateTime, order.CreatedAtUtc);
        Assert.Equal(23, (await db.Products.FindAsync(1))!.StockQuantity);
    }

    [Fact]
    public async Task CreateOrder_InsufficientStock_Throws()
    {
        var (service, _) = CreateService();
        var request = new CreateOrderRequest(1, [new(3, 50)]);

        await Assert.ThrowsAsync<OrderValidationException>(
            () => service.CreateOrderAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateOrder_UnknownCustomer_Throws()
    {
        var (service, _) = CreateService();
        var request = new CreateOrderRequest(999, [new(1, 1)]);

        await Assert.ThrowsAsync<OrderValidationException>(
            () => service.CreateOrderAsync(request, CancellationToken.None));
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
