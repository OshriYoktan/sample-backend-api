using System.ComponentModel.DataAnnotations;

namespace Orders.Api.Dtos;

public record CustomerDto(int Id, string Name, string Email);

public record ProductDto(int Id, string Name, decimal Price, int StockQuantity);

public record OrderDto(
    int Id,
    int CustomerId,
    DateTime CreatedAtUtc,
    string Status,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items);

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record CreateOrderRequest(
    [Range(1, int.MaxValue)] int CustomerId,
    [MinLength(1)] IReadOnlyList<CreateOrderItemRequest> Items);

public record CreateOrderItemRequest(
    [Range(1, int.MaxValue)] int ProductId,
    [Range(1, 100)] int Quantity);
