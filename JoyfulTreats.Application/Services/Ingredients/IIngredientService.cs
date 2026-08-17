using JoyfulTreats.Application.DTOs.Ingredients;

namespace JoyfulTreats.Application.Services.Ingredients;

public interface IIngredientService
{
    Task<IReadOnlyList<IngredientDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<IngredientDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IngredientDto> CreateAsync(CreateIngredientDto request, CancellationToken cancellationToken);

    Task<IngredientDto?> UpdateAsync(int id, UpdateIngredientDto request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
