using Microsoft.AspNetCore.Mvc;
using Orders.Api.Dtos;
using Orders.Api.Mapping;
using Orders.Api.Repositories;

namespace Orders.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductRepository products) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken ct) =>
        Ok((await products.GetAllAsync(ct)).Select(p => p.ToDto()).ToList());
}
