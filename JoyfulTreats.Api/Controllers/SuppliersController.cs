using JoyfulTreats.Application.DTOs.Suppliers;
using JoyfulTreats.Application.Services.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace JoyfulTreats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController(ISupplierService supplierService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SupplierDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await supplierService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var supplier = await supplierService.GetByIdAsync(id, cancellationToken);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create(CreateSupplierDto request, CancellationToken cancellationToken)
    {
        var supplier = await supplierService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SupplierDto>> Update(int id, UpdateSupplierDto request, CancellationToken cancellationToken)
    {
        var supplier = await supplierService.UpdateAsync(id, request, cancellationToken);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await supplierService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
