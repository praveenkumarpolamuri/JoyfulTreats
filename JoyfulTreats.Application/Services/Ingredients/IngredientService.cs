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
                // Get active cost from price history
                CostPerUnit = ingredient.PriceHistories
                    .Where(ph => ph.EffectiveTo == null)
                    .Select(ph => ph.UnitCost)
                    .FirstOrDefault(),
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
                // Get active cost from price history
                CostPerUnit = ingredient.PriceHistories
                    .Where(ph => ph.EffectiveTo == null)
                    .Select(ph => ph.UnitCost)
                    .FirstOrDefault(),
                IsActive = ingredient.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IngredientDto> CreateAsync(CreateIngredientDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || !UnitConversion.IsSupported(request.Unit) || request.CostPerUnit < 0)
            throw new ArgumentException("Name, a supported unit, and a non-negative cost per unit are required.");

        var now = DateTime.UtcNow;

        var ingredient = new Ingredient
        {
            Name = request.Name.Trim(),
            Unit = request.Unit.Trim(),
            CreatedAt = now,
            IsActive = true
        };

        // Add initial price history record
        ingredient.PriceHistories.Add(new IngredientPriceHistory
        {
            UnitCost = request.CostPerUnit,
            EffectiveFrom = now,
            EffectiveTo = null
        });

        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync(cancellationToken);

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Unit = ingredient.Unit,
            CostPerUnit = request.CostPerUnit,
            IsActive = ingredient.IsActive
        };
    }

    public async Task<IngredientDto?> UpdateAsync(int id, UpdateIngredientDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || !UnitConversion.IsSupported(request.Unit) || request.CostPerUnit < 0)
            throw new ArgumentException("Name, a supported unit, and a non-negative cost per unit are required.");

        var ingredient = await context.Ingredients
            .Include(i => i.PriceHistories)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (ingredient is null) return null;

        var isUsedInRecipes = await context.RecipeIngredients
            .AnyAsync(item => item.IngredientId == id, cancellationToken);
        if (isUsedInRecipes)
            UnitConversion.Convert(1, ingredient.Unit, request.Unit);

        var now = DateTime.UtcNow;

        // Find active price history record
        var currentPriceHistory = ingredient.PriceHistories.FirstOrDefault(ph => ph.EffectiveTo == null);

        // If price changed (or no initial history exists), close old record and create a new one
        if (currentPriceHistory == null || currentPriceHistory.UnitCost != request.CostPerUnit)
        {
            if (currentPriceHistory != null)
            {
                currentPriceHistory.EffectiveTo = now;
            }

            ingredient.PriceHistories.Add(new IngredientPriceHistory
            {
                IngredientId = ingredient.Id,
                UnitCost = request.CostPerUnit,
                EffectiveFrom = now,
                EffectiveTo = null
            });
        }

        ingredient.Name = request.Name.Trim();
        ingredient.Unit = request.Unit.Trim();
        ingredient.IsActive = request.IsActive;
        ingredient.UpdatedAt = now;

        await context.SaveChangesAsync(cancellationToken);

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Unit = ingredient.Unit,
            CostPerUnit = request.CostPerUnit,
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