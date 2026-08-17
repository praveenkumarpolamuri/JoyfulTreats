using JoyfulTreats.Application.DTOs.Purchases;
using JoyfulTreats.Application.Interfaces.Persistence;
using JoyfulTreats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoyfulTreats.Application.Services.Purchases;

public class PurchaseService(IApplicationDbContext context) : IPurchaseService
{
    private const string Pending = "PENDING";
    private const string Received = "RECEIVED";
    private const string Cancelled = "CANCELLED";

    public async Task<IReadOnlyList<PurchaseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var purchases = await context.Purchases
            .AsNoTracking()
            .Include(purchase => purchase.Supplier)
            .Include(purchase => purchase.Items)
                .ThenInclude(item => item.Ingredient)
            .OrderByDescending(purchase => purchase.PurchaseDate)
            .ThenByDescending(purchase => purchase.Id)
            .ToListAsync(cancellationToken);

        return purchases.Select(ToDto).ToList();
    }

    public async Task<PurchaseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var purchase = await context.Purchases
            .AsNoTracking()
            .Include(purchase => purchase.Supplier)
            .Include(purchase => purchase.Items)
                .ThenInclude(item => item.Ingredient)
            .FirstOrDefaultAsync(
                purchase => purchase.Id == id,
                cancellationToken);

        return purchase is null ? null : ToDto(purchase);
    }

    public async Task<PurchaseDto> CreateAsync(
        CreatePurchaseDto request,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request.SupplierId, request.PurchaseDate, request.Items);

        var supplierExists = await context.Suppliers
            .AnyAsync(
                supplier => supplier.Id == request.SupplierId &&
                            supplier.IsActive,
                cancellationToken);

        if (!supplierExists)
            throw new InvalidOperationException(
                "The specified active supplier does not exist.");

        var ingredientIds = request.Items
            .Select(item => item.IngredientId)
            .Distinct()
            .ToArray();

        if (ingredientIds.Length != request.Items.Count)
            throw new ArgumentException(
                "An ingredient cannot appear more than once in a purchase.");

        var ingredients = await context.Ingredients
            .Where(ingredient =>
                ingredient.IsActive &&
                ingredientIds.Contains(ingredient.Id))
            .ToListAsync(cancellationToken);

        if (ingredients.Count != ingredientIds.Length)
            throw new InvalidOperationException(
                "One or more ingredients do not exist or are inactive.");

        var ingredientLookup = ingredients.ToDictionary(
            ingredient => ingredient.Id);

        var purchase = new Purchase
        {
            SupplierId = request.SupplierId,
            PurchaseDate = request.PurchaseDate,
            InvoiceNumber = request.InvoiceNumber?.Trim(),
            Status = Pending,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in request.Items)
        {
            var ingredient = ingredientLookup[item.IngredientId];

            var totalCost = item.Quantity * item.UnitCost;

            purchase.Items.Add(new PurchaseItem
            {
                IngredientId = ingredient.Id,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost,
                TotalCost = totalCost
            });
        }

        purchase.TotalAmount = purchase.Items.Sum(item => item.TotalCost);

        context.Purchases.Add(purchase);

        await context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(purchase.Id, cancellationToken)
            ?? throw new InvalidOperationException(
                "Purchase could not be retrieved after creation.");
    }

    public async Task<PurchaseDto?> UpdateAsync(
        int id,
        UpdatePurchaseDto request,
        CancellationToken cancellationToken)
    {
        ValidateRequest(
            request.SupplierId,
            request.PurchaseDate,
            request.Items);

        var purchase = await context.Purchases
            .Include(item => item.Items)
            .FirstOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (purchase is null)
            return null;

        if (purchase.Status != Pending)
        {
            throw new InvalidOperationException(
                "Only pending purchases can be updated.");
        }

        var supplierExists = await context.Suppliers
            .AnyAsync(
                supplier => supplier.Id == request.SupplierId &&
                            supplier.IsActive,
                cancellationToken);

        if (!supplierExists)
            throw new InvalidOperationException(
                "The specified active supplier does not exist.");

        var ingredientIds = request.Items
            .Select(item => item.IngredientId)
            .Distinct()
            .ToArray();

        if (ingredientIds.Length != request.Items.Count)
            throw new ArgumentException(
                "An ingredient cannot appear more than once in a purchase.");

        var ingredients = await context.Ingredients
            .Where(ingredient =>
                ingredient.IsActive &&
                ingredientIds.Contains(ingredient.Id))
            .ToListAsync(cancellationToken);

        if (ingredients.Count != ingredientIds.Length)
            throw new InvalidOperationException(
                "One or more ingredients do not exist or are inactive.");

        context.PurchaseItems.RemoveRange(purchase.Items);

        purchase.SupplierId = request.SupplierId;
        purchase.PurchaseDate = request.PurchaseDate;
        purchase.InvoiceNumber = request.InvoiceNumber?.Trim();
        purchase.UpdatedAt = DateTime.UtcNow;

        purchase.Items = request.Items.Select(item =>
            new PurchaseItem
            {
                IngredientId = item.IngredientId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost,
                TotalCost = item.Quantity * item.UnitCost
            }).ToList();

        purchase.TotalAmount = purchase.Items.Sum(item => item.TotalCost);

        await context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<PurchaseDto?> ReceiveAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var purchase = await context.Purchases
            .Include(item => item.Items)
                .ThenInclude(item => item.Ingredient)
            .FirstOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (purchase is null)
            return null;

        if (purchase.Status == Received)
        {
            throw new InvalidOperationException(
                "This purchase has already been received.");
        }

        if (purchase.Status == Cancelled)
        {
            throw new InvalidOperationException(
                "A cancelled purchase cannot be received.");
        }

        foreach (var item in purchase.Items)
        {
            var stock = await context.InventoryStocks
                .FirstOrDefaultAsync(
                    stock => stock.IngredientId == item.IngredientId,
                    cancellationToken);

            if (stock is null)
            {
                stock = new InventoryStock
                {
                    IngredientId = item.IngredientId,
                    Quantity = item.Quantity,
                    ReorderLevel = 0,
                    UpdatedAt = DateTime.UtcNow
                };

                context.InventoryStocks.Add(stock);
            }
            else
            {
                stock.Quantity += item.Quantity;
                stock.UpdatedAt = DateTime.UtcNow;
            }

            context.InventoryTransactions.Add(
                new InventoryTransaction
                {
                    IngredientId = item.IngredientId,
                    TransactionType = "PURCHASE",
                    Quantity = item.Quantity,
                    CreatedAt = DateTime.UtcNow
                });
        }

        purchase.Status = Received;
        purchase.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> CancelAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var purchase = await context.Purchases
            .FirstOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (purchase is null)
            return false;

        if (purchase.Status == Received)
        {
            throw new InvalidOperationException(
                "A received purchase cannot be cancelled.");
        }

        if (purchase.Status == Cancelled)
            return true;

        purchase.Status = Cancelled;
        purchase.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void ValidateRequest(
        int supplierId,
        DateOnly purchaseDate,
        IReadOnlyCollection<CreatePurchaseItemDto> items)
    {
        if (supplierId <= 0)
            throw new ArgumentException(
                "A valid supplier is required.");

        if (purchaseDate == default)
            throw new ArgumentException(
                "Purchase date is required.");

        if (items.Count == 0)
            throw new ArgumentException(
                "At least one purchase item is required.");

        if (items.Any(item =>
                item.IngredientId <= 0 ||
                item.Quantity <= 0 ||
                item.UnitCost < 0))
        {
            throw new ArgumentException(
                "Each item must have a valid ingredient, positive quantity, and non-negative unit cost.");
        }
    }

    private static PurchaseDto ToDto(Purchase purchase)
    {
        return new PurchaseDto
        {
            Id = purchase.Id,
            SupplierId = purchase.SupplierId,
            SupplierName = purchase.Supplier.Name,
            PurchaseDate = purchase.PurchaseDate,
            InvoiceNumber = purchase.InvoiceNumber,
            TotalAmount = purchase.TotalAmount,
            Status = purchase.Status,
            Items = purchase.Items
                .Select(item => new PurchaseItemDto
                {
                    Id = item.Id,
                    IngredientId = item.IngredientId,
                    IngredientName = item.Ingredient.Name,
                    Unit = item.Ingredient.Unit,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    TotalCost = item.TotalCost
                })
                .ToList()
        };
    }
}