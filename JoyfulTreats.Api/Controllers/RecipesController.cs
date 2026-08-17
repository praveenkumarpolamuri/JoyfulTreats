using JoyfulTreats.Application.DTOs.Recipes;
using JoyfulTreats.Application.Services.Recipes;
using Microsoft.AspNetCore.Mvc;

namespace JoyfulTreats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController(IRecipeService recipeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RecipeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await recipeService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var recipe = await recipeService.GetByIdAsync(id, cancellationToken);
        return recipe is null ? NotFound() : Ok(recipe);
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Create(CreateRecipeDto request, CancellationToken cancellationToken)
    {
        var recipe = await recipeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RecipeDto>> Update(int id, UpdateRecipeDto request, CancellationToken cancellationToken)
    {
        var recipe = await recipeService.UpdateAsync(id, request, cancellationToken);
        return recipe is null ? NotFound() : Ok(recipe);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await recipeService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
