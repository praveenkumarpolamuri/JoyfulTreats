using JoyfulTreats.Application.DTOs.Recipes;

namespace JoyfulTreats.Application.Services.Recipes;

public interface IRecipeService
{
    Task<IReadOnlyList<RecipeDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<RecipeDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<RecipeDto> CreateAsync(CreateRecipeDto request, CancellationToken cancellationToken);

    Task<RecipeDto?> UpdateAsync(int id, UpdateRecipeDto request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
