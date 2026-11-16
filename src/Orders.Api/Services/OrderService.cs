using Orders.Api.Dtos;
using Orders.Api.Entities;
using Orders.Api.Mapping;
using Orders.Api.Repositories;

namespace Orders.Api.Services;

public interface IOrderService
{
    Task<OrderDto?> GetOrderAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<OrderDto>?> GetCustomerOrdersAsync(int customerId, CancellationToken ct);
    Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct);
}

/// <summary>Business-rule violation; controllers translate it to 400 Bad Request.</summary>
public class OrderValidationException(string message) : Exception(message);

public class OrderService(
    IOrderRepository orders,
    ICustomerRepository customers,
    IProductRepository products,
    TimeProvider clock) : IOrderService
{
    public async Task<OrderDto?> GetOrderAsync(int id, CancellationToken ct) =>
        (await orders.GetByIdAsync(id, ct))?.ToDto();

    /// <returns>null when the customer does not exist.</returns>
    public async Task<IReadOnlyList<OrderDto>?> GetCustomerOrdersAsync(int customerId, CancellationToken ct)
    {
        if (!await customers.ExistsAsync(customerId, ct))
            return null;

        var customerOrders = await orders.GetByCustomerAsync(customerId, ct);
        return customerOrders.Select(o => o.ToDto()).ToList();
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct)
    {
        if (!await customers.ExistsAsync(request.CustomerId, ct))
            throw new OrderValidationException($"Customer {request.CustomerId} does not exist.");

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var productsById = await products.GetByIdsAsync(productIds, ct);

        var order = new Order
        {
            CustomerId = request.CustomerId,
            CreatedAtUtc = clock.GetUtcNow().UtcDateTime,
            Status = OrderStatus.Pending
        };

        foreach (var item in request.Items)
        {
            if (!productsById.TryGetValue(item.ProductId, out var product))
                throw new OrderValidationException($"Product {item.ProductId} does not exist.");
            if (product.StockQuantity < item.Quantity)
                throw new OrderValidationException($"Insufficient stock for product {product.Id}.");

            product.StockQuantity -= item.Quantity;
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        await orders.AddAsync(order, ct);
        await orders.SaveChangesAsync(ct);
        return order.ToDto();
    }
}
