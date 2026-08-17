using JoyfulTreats.Application.DTOs.Inventory;
using JoyfulTreats.Application.Services.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace JoyfulTreats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventoryStockDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await inventoryService.GetAllAsync(cancellationToken));

    [HttpGet("{ingredientId:int}")]
    public async Task<ActionResult<InventoryStockDto>> GetByIngredient(int ingredientId, CancellationToken cancellationToken)
    {
        var stock = await inventoryService.GetByIngredientIdAsync(ingredientId, cancellationToken);
        return stock is null ? NotFound() : Ok(stock);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IReadOnlyList<InventoryStockDto>>> GetLowStock(CancellationToken cancellationToken) =>
        Ok(await inventoryService.GetLowStockAsync(cancellationToken));

    [HttpPost("adjust")]
    public async Task<ActionResult<InventoryStockDto>> Adjust(AdjustInventoryDto request, CancellationToken cancellationToken)
    {
        var stock = await inventoryService.AdjustAsync(request, cancellationToken);
        return Ok(stock);
    }
}
