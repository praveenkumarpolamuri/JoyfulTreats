using JoyfulTreats.Application.DTOs.Sales;
using JoyfulTreats.Application.Services.Sales;
using Microsoft.AspNetCore.Mvc;

namespace JoyfulTreats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController(ISaleService saleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SaleDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await saleService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SaleDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var sale = await saleService.GetByIdAsync(id, cancellationToken);
        return sale is null ? NotFound() : Ok(sale);
    }

    [HttpPost]
    public async Task<ActionResult<SaleDto>> Create(CreateSaleDto request, CancellationToken cancellationToken)
    {
        var sale = await saleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SaleDto>> Update(int id, UpdateSaleDto request, CancellationToken cancellationToken)
    {
        var sale = await saleService.UpdateAsync(id, request, cancellationToken);
        return sale is null ? NotFound() : Ok(sale);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await saleService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
