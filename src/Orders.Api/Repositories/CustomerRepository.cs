using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Dtos;
using Orders.Api.Entities;

namespace Orders.Api.Repositories;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct);
    Task<Customer?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> ExistsAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<CustomerOrderCountDto>> GetOrderCountsAsync(CancellationToken ct);
}

public class CustomerRepository(OrdersDbContext db) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct) =>
        await db.Customers.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public Task<Customer?> GetByIdAsync(int id, CancellationToken ct) =>
        db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        db.Customers.AnyAsync(c => c.Id == id, ct);

    /// <summary>
    /// Order count per customer (customers without orders get 0), sorted by name.
    /// Translated by EF Core into a single SQL statement, so there are no per-customer queries.
    /// </summary>
    public async Task<IReadOnlyList<CustomerOrderCountDto>> GetOrderCountsAsync(CancellationToken ct) =>
        await db.Customers.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CustomerOrderCountDto(c.Id, c.Name, c.Orders.Count))
            .ToListAsync(ct);
}
