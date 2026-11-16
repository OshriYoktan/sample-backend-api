using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Entities;

namespace Orders.Api.Repositories;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct);
    Task<Customer?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> ExistsAsync(int id, CancellationToken ct);
}

public class CustomerRepository(OrdersDbContext db) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct) =>
        await db.Customers.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public Task<Customer?> GetByIdAsync(int id, CancellationToken ct) =>
        db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        db.Customers.AnyAsync(c => c.Id == id, ct);
}
