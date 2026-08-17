using JoyfulTreats.Application.DTOs.Purchases;
using JoyfulTreats.Application.Services.Purchases;
using Microsoft.AspNetCore.Mvc;

namespace JoyfulTreats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController(IPurchaseService purchaseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PurchaseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var purchases = await purchaseService.GetAllAsync(cancellationToken);

        return Ok(purchases);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PurchaseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var purchase = await purchaseService.GetByIdAsync(
            id,
            cancellationToken);

        if (purchase is null)
            return NotFound();

        return Ok(purchase);
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseDto>> Create(
        CreatePurchaseDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var purchase = await purchaseService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = purchase.Id },
                purchase);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PurchaseDto>> Update(
        int id,
        UpdatePurchaseDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var purchase = await purchaseService.UpdateAsync(
                id,
                request,
                cancellationToken);

            if (purchase is null)
                return NotFound();

            return Ok(purchase);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/receive")]
    public async Task<ActionResult<PurchaseDto>> Receive(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var purchase = await purchaseService.ReceiveAsync(
                id,
                cancellationToken);

            if (purchase is null)
                return NotFound();

            return Ok(purchase);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var cancelled = await purchaseService.CancelAsync(
                id,
                cancellationToken);

            if (!cancelled)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}