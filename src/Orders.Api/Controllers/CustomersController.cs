using Microsoft.AspNetCore.Mvc;
using Orders.Api.Dtos;
using Orders.Api.Mapping;
using Orders.Api.Repositories;
using Orders.Api.Services;

namespace Orders.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(ICustomerRepository customers, IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll(CancellationToken ct) =>
        Ok((await customers.GetAllAsync(ct)).Select(c => c.ToDto()).ToList());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id, CancellationToken ct)
    {
        var customer = await customers.GetByIdAsync(id, ct);
        return customer is null ? NotFound() : Ok(customer.ToDto());
    }

    /// <summary>All orders of a customer, newest first.</summary>
    [HttpGet("{id:int}/orders")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders(int id, CancellationToken ct)
    {
        var result = await orderService.GetCustomerOrdersAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
