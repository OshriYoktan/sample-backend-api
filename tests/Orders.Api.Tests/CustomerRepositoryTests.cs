using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Entities;
using Orders.Api.Repositories;

namespace Orders.Api.Tests;

public class CustomerRepositoryTests
{
    private static (CustomerRepository Repository, OrdersDbContext Db) CreateRepository(bool seed = true)
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new OrdersDbContext(options);
        if (seed)
        {
            db.Database.EnsureCreated(); // applies HasData seed
        }

        return (new CustomerRepository(db), db);
    }

    [Fact]
    public async Task GetOrderCounts_SeedData_ReturnsCountPerCustomer()
    {
        var (repository, _) = CreateRepository();

        var counts = await repository.GetOrderCountsAsync(CancellationToken.None);

        Assert.Collection(counts,
            c =>
            {
                Assert.Equal(2, c.CustomerId);
                Assert.Equal("Avi Cohen", c.CustomerName);
                Assert.Equal(1, c.OrderCount);
            },
            c =>
            {
                Assert.Equal(1, c.CustomerId);
                Assert.Equal("Dana Levi", c.CustomerName);
                Assert.Equal(2, c.OrderCount);
            });
    }

    [Fact]
    public async Task GetOrderCounts_CustomerWithoutOrders_ReturnsZero()
    {
        var (repository, db) = CreateRepository();
        db.Customers.Add(new Customer
        {
            Id = 3,
            Name = "Noa Mizrahi",
            Email = "noa@example.com",
            CreatedAtUtc = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        });
        await db.SaveChangesAsync();

        var counts = await repository.GetOrderCountsAsync(CancellationToken.None);

        Assert.Equal(3, counts.Count);
        var noa = Assert.Single(counts, c => c.CustomerId == 3);
        Assert.Equal(0, noa.OrderCount);
    }

    [Fact]
    public async Task GetOrderCounts_NoCustomers_ReturnsEmptyList()
    {
        var (repository, _) = CreateRepository(seed: false);

        var counts = await repository.GetOrderCountsAsync(CancellationToken.None);

        Assert.Empty(counts);
    }
}
