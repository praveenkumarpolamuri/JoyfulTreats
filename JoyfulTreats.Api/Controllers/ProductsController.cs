using JoyfulTreats.Application.DTOs.Products;
using JoyfulTreats.Application.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace JoyfulTreats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await _productService
            .GetAllAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _productService
            .GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductDto request,
        CancellationToken cancellationToken)
    {
        var product = await _productService
            .CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(
        int id,
        UpdateProductDto request,
        CancellationToken cancellationToken)
    {
        var product = await _productService
            .UpdateAsync(id, request, cancellationToken);
               return CreatedAtAction(
            nameof(GetById),
            new { id = product?.Id },
            product);
    }

[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(
    int id,
    CancellationToken cancellationToken)
{
    var deleted = await _productService.DeleteAsync(
        id,
        cancellationToken);

    if (!deleted)
    {
        return NotFound();
    }

    return NoContent();
}

    
}