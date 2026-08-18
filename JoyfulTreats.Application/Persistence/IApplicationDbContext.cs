using JoyfulTreats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoyfulTreats.Application.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Ingredient> Ingredients { get; }
    DbSet<IngredientPriceHistory> IngredientPriceHistories { get; } // Add here
    DbSet<Recipe> Recipes { get; }
    DbSet<RecipeIngredient> RecipeIngredients { get; }
    DbSet<Sale> Sales { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<Purchase> Purchases { get; }
    DbSet<PurchaseItem> PurchaseItems { get; }
    DbSet<InventoryStock> InventoryStocks { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<ProductionBatch> ProductionBatches { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<ProductStock> ProductStocks { get; }
    DbSet<ProductInventoryTransaction> ProductInventoryTransactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}