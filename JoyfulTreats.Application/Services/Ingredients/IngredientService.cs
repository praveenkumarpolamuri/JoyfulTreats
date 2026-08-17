using JoyfulTreats.Application.DTOs.Ingredients;
using JoyfulTreats.Application.Interfaces.Persistence;
using JoyfulTreats.Application.Services;
using JoyfulTreats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoyfulTreats.Application.Services.Ingredients;

public class IngredientService(IApplicationDbContext context) : IIngredientService
{
    public async Task<IReadOnlyList<IngredientDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.Ingredients.AsNoTracking()
            .Where(ingredient => ingredient.IsActive)
            .OrderBy(ingredient => ingredient.Name)
            .Select(ingredient => new IngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Unit = ingredient.Unit,
                CostPerUnit = ingredient.CostPerUnit,
                IsActive = ingredient.IsActive
            })
            .ToListAsync(cancellationToken);

    public async Task<IngredientDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await context.Ingredients.AsNoTracking()
            .Where(ingredient => ingredient.Id == id)
            .Select(ingredient => new IngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Unit = ingredient.Unit,
                CostPerUnit = ingredient.CostPerUnit,
                IsActive = ingredient.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IngredientDto> CreateAsync(CreateIngredientDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || !UnitConversion.IsSupported(request.Unit) || request.CostPerUnit < 0)
            throw new ArgumentException("Name, a supported unit, and a non-negative cost per unit are required.");

        var ingredient = new Ingredient
        {
            Name = request.Name.Trim(),
            Unit = request.Unit.Trim(),
            CostPerUnit = request.CostPerUnit,
            CreatedAt = DateTime.UtcNow
        };

        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync(cancellationToken);

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Unit = ingredient.Unit,
            CostPerUnit = ingredient.CostPerUnit,
            IsActive = ingredient.IsActive
        };
    }

    public async Task<IngredientDto?> UpdateAsync(int id, UpdateIngredientDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || !UnitConversion.IsSupported(request.Unit) || request.CostPerUnit < 0)
            throw new ArgumentException("Name, a supported unit, and a non-negative cost per unit are required.");

        var ingredient = await context.Ingredients.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (ingredient is null) return null;

        var isUsedInRecipes = await context.RecipeIngredients
            .AnyAsync(item => item.IngredientId == id, cancellationToken);
        if (isUsedInRecipes)
            UnitConversion.Convert(1, ingredient.Unit, request.Unit);

        ingredient.Name = request.Name.Trim();
        ingredient.Unit = request.Unit.Trim();
        ingredient.CostPerUnit = request.CostPerUnit;
        ingredient.IsActive = request.IsActive;
        ingredient.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Unit = ingredient.Unit,
            CostPerUnit = ingredient.CostPerUnit,
            IsActive = ingredient.IsActive
        };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var ingredient = await context.Ingredients.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (ingredient is null) return false;

        ingredient.IsActive = false;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
