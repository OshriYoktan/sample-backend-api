using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Entities;

namespace Orders.Api.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);
    Task<Dictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct);
}

public class ProductRepository(OrdersDbContext db) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct) =>
        await db.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);

    public Task<Dictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct) =>
        db.Products.Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id, ct);
}
