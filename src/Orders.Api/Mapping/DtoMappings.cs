using Orders.Api.Dtos;
using Orders.Api.Entities;

namespace Orders.Api.Mapping;

public static class DtoMappings
{
    public static CustomerDto ToDto(this Customer customer) =>
        new(customer.Id, customer.Name, customer.Email);

    public static ProductDto ToDto(this Product product) =>
        new(product.Id, product.Name, product.Price, product.StockQuantity);

    public static OrderDto ToDto(this Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.CreatedAtUtc,
            order.Status.ToString(),
            order.TotalAmount,
            order.Items
                .Select(i => new OrderItemDto(i.ProductId, i.Product?.Name ?? string.Empty, i.Quantity, i.UnitPrice))
                .ToList());
}
