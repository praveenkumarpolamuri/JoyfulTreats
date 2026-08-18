using JoyfulTreats.Application.DTOs.Recipes;
using JoyfulTreats.Application.Interfaces.Persistence;
using JoyfulTreats.Application.Services;
using JoyfulTreats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoyfulTreats.Application.Services.Recipes;

public class RecipeService(IApplicationDbContext context) : IRecipeService
{
    public async Task<IReadOnlyList<RecipeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var recipes = await context.Recipes.AsNoTracking()
            .Include(recipe => recipe.Product)
            .Include(recipe => recipe.RecipeIngredients)
                .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
                    .ThenInclude(ingredient => ingredient.PriceHistories) // Eager load active prices
            .OrderBy(recipe => recipe.Product.Name)
            .ToListAsync(cancellationToken);

        return recipes.Select(ToDto).ToList();
    }

    public async Task<RecipeDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await context.Recipes.AsNoTracking()
            .Include(recipe => recipe.Product)
            .Include(recipe => recipe.RecipeIngredients)
                .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
                    .ThenInclude(ingredient => ingredient.PriceHistories) // Eager load active prices
            .Where(recipe => recipe.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return recipe is null ? null : ToDto(recipe);
    }

    public async Task<RecipeDto> CreateAsync(CreateRecipeDto request, CancellationToken cancellationToken)
    {
        if (request.ProductId <= 0 || request.YieldQuantity <= 0 || request.Ingredients.Count == 0)
            throw new ArgumentException("A product, positive batch yield, and at least one ingredient are required.");

        if (request.Ingredients.Any(item => item.IngredientId <= 0 || item.Quantity <= 0 || !UnitConversion.IsSupported(item.Unit)) ||
            request.Ingredients.Select(item => item.IngredientId).Distinct().Count() != request.Ingredients.Count)
            throw new ArgumentException("Each ingredient must be included once with a positive quantity.");

        var productExists = await context.Products.AnyAsync(product => product.Id == request.ProductId && product.IsActive, cancellationToken);
        if (!productExists) throw new InvalidOperationException("The specified active product does not exist.");

        var recipeExists = await context.Recipes.AnyAsync(recipe => recipe.ProductId == request.ProductId, cancellationToken);
        if (recipeExists) throw new InvalidOperationException("This product already has a recipe.");

        var ingredientIds = request.Ingredients.Select(item => item.IngredientId).ToArray();
        var availableIngredients = await context.Ingredients
            .Where(ingredient => ingredient.IsActive && ingredientIds.Contains(ingredient.Id))
            .Select(ingredient => new { ingredient.Id, ingredient.Unit })
            .ToListAsync(cancellationToken);

        if (availableIngredients.Count != ingredientIds.Length)
            throw new InvalidOperationException("One or more ingredients do not exist or are inactive.");

        foreach (var recipeIngredient in request.Ingredients)
        {
            var purchaseUnit = availableIngredients
                .Single(ingredient => ingredient.Id == recipeIngredient.IngredientId)
                .Unit;

            UnitConversion.Convert(1, recipeIngredient.Unit, purchaseUnit);
        }

        var recipe = new Recipe
        {
            ProductId = request.ProductId,
            YieldQuantity = request.YieldQuantity,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = request.Ingredients.Select(item => new RecipeIngredient
            {
                IngredientId = item.IngredientId,
                Quantity = item.Quantity,
                Unit = item.Unit.Trim()
            }).ToList()
        };

        context.Recipes.Add(recipe);
        await context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(recipe.Id, cancellationToken)
            ?? throw new InvalidOperationException("Recipe could not be retrieved after creation.");
    }

    public async Task<RecipeDto?> UpdateAsync(int id, UpdateRecipeDto request, CancellationToken cancellationToken)
    {
        if (request.ProductId <= 0 || request.YieldQuantity <= 0 || request.Ingredients.Count == 0)
            throw new ArgumentException("A product, positive batch yield, and at least one ingredient are required.");

        if (request.Ingredients.Any(item => item.IngredientId <= 0 || item.Quantity <= 0 || !UnitConversion.IsSupported(item.Unit)) ||
            request.Ingredients.Select(item => item.IngredientId).Distinct().Count() != request.Ingredients.Count)
            throw new ArgumentException("Each ingredient must be included once with a positive quantity.");

        var recipe = await context.Recipes
            .Include(item => item.RecipeIngredients)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (recipe is null) return null;

        var productExists = await context.Products.AnyAsync(product => product.Id == request.ProductId && product.IsActive, cancellationToken);
        if (!productExists) throw new InvalidOperationException("The specified active product does not exist.");

        var anotherRecipeExists = await context.Recipes.AnyAsync(item => item.ProductId == request.ProductId && item.Id != id, cancellationToken);
        if (anotherRecipeExists) throw new InvalidOperationException("This product already has a recipe.");

        var ingredientIds = request.Ingredients.Select(item => item.IngredientId).ToArray();
        var ingredients = await context.Ingredients
            .Where(item => item.IsActive && ingredientIds.Contains(item.Id))
            .Select(item => new { item.Id, item.Unit })
            .ToListAsync(cancellationToken);
        if (ingredients.Count != ingredientIds.Length)
            throw new InvalidOperationException("One or more ingredients do not exist or are inactive.");

        foreach (var line in request.Ingredients)
            UnitConversion.Convert(1, line.Unit, ingredients.Single(item => item.Id == line.IngredientId).Unit);

        context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
        recipe.ProductId = request.ProductId;
        recipe.YieldQuantity = request.YieldQuantity;
        recipe.UpdatedAt = DateTime.UtcNow;
        recipe.RecipeIngredients = request.Ingredients.Select(line => new RecipeIngredient
        {
            IngredientId = line.IngredientId,
            Quantity = line.Quantity,
            Unit = line.Unit.Trim()
        }).ToList();

        await context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await context.Recipes.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (recipe is null) return false;

        context.Recipes.Remove(recipe);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static RecipeDto ToDto(Recipe recipe)
    {
        var totalCost = recipe.RecipeIngredients.Sum(item =>
        {
            var activeCost = item.Ingredient.PriceHistories
                .FirstOrDefault(ph => ph.EffectiveTo == null)?.UnitCost ?? 0m;

            return UnitConversion.CalculateCost(item.Quantity, item.Unit, activeCost, item.Ingredient.Unit);
        });

        var costPerItem = recipe.YieldQuantity > 0 ? totalCost / recipe.YieldQuantity : 0;
        var grossProfit = recipe.Product.SellingPrice - costPerItem;
        var marginPercentage = recipe.Product.SellingPrice > 0 ? (grossProfit / recipe.Product.SellingPrice) * 100 : 0;

        return new RecipeDto
        {
            Id = recipe.Id,
            ProductId = recipe.ProductId,
            ProductName = recipe.Product.Name,
            YieldQuantity = recipe.YieldQuantity,
            TotalCost = totalCost,
            CostPerItem = costPerItem,
            SellingPrice = recipe.Product.SellingPrice,
            GrossProfit = grossProfit,
            MarginPercentage = marginPercentage,
            Ingredients = recipe.RecipeIngredients.Select(item =>
            {
                var activeCost = item.Ingredient.PriceHistories
                    .FirstOrDefault(ph => ph.EffectiveTo == null)?.UnitCost ?? 0m;

                return new RecipeIngredientDto
                {
                    IngredientId = item.IngredientId,
                    IngredientName = item.Ingredient.Name,
                    Unit = item.Unit,
                    Quantity = item.Quantity,
                    Cost = UnitConversion.CalculateCost(item.Quantity, item.Unit, activeCost, item.Ingredient.Unit)
                };
            }).ToList()
        };
    }
}