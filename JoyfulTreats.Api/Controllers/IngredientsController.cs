using JoyfulTreats.Application.DTOs.Ingredients;
using JoyfulTreats.Application.Services.Ingredients;
using Microsoft.AspNetCore.Mvc;

namespace JoyfulTreats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController(IIngredientService ingredientService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<IngredientDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await ingredientService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IngredientDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var ingredient = await ingredientService.GetByIdAsync(id, cancellationToken);
        return ingredient is null ? NotFound() : Ok(ingredient);
    }

    [HttpPost]
    public async Task<ActionResult<IngredientDto>> Create(CreateIngredientDto request, CancellationToken cancellationToken)
    {
        var ingredient = await ingredientService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<IngredientDto>> Update(int id, UpdateIngredientDto request, CancellationToken cancellationToken)
    {
        var ingredient = await ingredientService.UpdateAsync(id, request, cancellationToken);
        return ingredient is null ? NotFound() : Ok(ingredient);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await ingredientService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
