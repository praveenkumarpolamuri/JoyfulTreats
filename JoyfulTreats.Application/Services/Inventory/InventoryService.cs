using JoyfulTreats.Application.DTOs.Inventory;
using JoyfulTreats.Application.Interfaces.Persistence;
using JoyfulTreats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoyfulTreats.Application.Services.Inventory;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryStockDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<InventoryStockDto?> GetByIngredientIdAsync(int ingredientId, CancellationToken cancellationToken);

    Task<InventoryStockDto> AdjustAsync(AdjustInventoryDto request, CancellationToken cancellationToken);

    Task<IReadOnlyList<InventoryStockDto>> GetLowStockAsync(CancellationToken cancellationToken);
}

public class InventoryService(IApplicationDbContext context) : IInventoryService
{
    public async Task<IReadOnlyList<InventoryStockDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.InventoryStocks
            .AsNoTracking()
            .Include(s => s.Ingredient)
            .OrderBy(s => s.Ingredient.Name)
            .Select(s => new InventoryStockDto
            {
                Id = s.Id,
                IngredientId = s.IngredientId,
                IngredientName = s.Ingredient.Name,
                Unit = s.Ingredient.Unit,
                Quantity = s.Quantity,
                ReorderLevel = s.ReorderLevel
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryStockDto?> GetByIngredientIdAsync(int ingredientId, CancellationToken cancellationToken)
    {
        return await context.InventoryStocks
            .AsNoTracking()
            .Include(s => s.Ingredient)
            .Where(s => s.IngredientId == ingredientId)
            .Select(s => new InventoryStockDto
            {
                Id = s.Id,
                IngredientId = s.IngredientId,
                IngredientName = s.Ingredient.Name,
                Unit = s.Ingredient.Unit,
                Quantity = s.Quantity,
                ReorderLevel = s.ReorderLevel
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InventoryStockDto> AdjustAsync(AdjustInventoryDto request, CancellationToken cancellationToken)
    {
        if (request.Quantity == 0)
            throw new ArgumentException("Adjustment quantity cannot be zero.");

        var stock = await context.InventoryStocks
            .Include(s => s.Ingredient)
            .FirstOrDefaultAsync(s => s.IngredientId == request.IngredientId, cancellationToken);

        if (stock is null)
        {
            // Create new stock if it doesn't exist
            var ingredient = await context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == request.IngredientId, cancellationToken);

            if (ingredient is null)
                throw new InvalidOperationException("Ingredient does not exist.");

            stock = new InventoryStock
            {
                IngredientId = request.IngredientId,
                Quantity = request.Quantity,
                ReorderLevel = 0,
                UpdatedAt = DateTime.UtcNow
            };

            context.InventoryStocks.Add(stock);
        }
        else
        {
            stock.Quantity += request.Quantity;
            stock.UpdatedAt = DateTime.UtcNow;
        }

        // Record transaction
        var transaction = new InventoryTransaction
        {
            IngredientId = request.IngredientId,
            TransactionType = request.TransactionType,
            Quantity = request.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        context.InventoryTransactions.Add(transaction);
        await context.SaveChangesAsync(cancellationToken);

        return new InventoryStockDto
        {
            Id = stock.Id,
            IngredientId = stock.IngredientId,
            IngredientName = stock.Ingredient.Name,
            Unit = stock.Ingredient.Unit,
            Quantity = stock.Quantity,
            ReorderLevel = stock.ReorderLevel
        };
    }

    public async Task<IReadOnlyList<InventoryStockDto>> GetLowStockAsync(CancellationToken cancellationToken)
    {
        return await context.InventoryStocks
            .AsNoTracking()
            .Include(s => s.Ingredient)
            .Where(s => s.Quantity <= s.ReorderLevel)
            .OrderBy(s => s.Ingredient.Name)
            .Select(s => new InventoryStockDto
            {
                Id = s.Id,
                IngredientId = s.IngredientId,
                IngredientName = s.Ingredient.Name,
                Unit = s.Ingredient.Unit,
                Quantity = s.Quantity,
                ReorderLevel = s.ReorderLevel
            })
            .ToListAsync(cancellationToken);
    }
}
