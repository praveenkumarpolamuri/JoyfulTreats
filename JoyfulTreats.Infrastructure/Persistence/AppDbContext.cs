using JoyfulTreats.Application.Interfaces.Persistence;
using JoyfulTreats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoyfulTreats.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();    

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<Purchase> Purchases => Set<Purchase>();

    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();

    public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();

    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<ProductionBatch> ProductionBatches => Set<ProductionBatch>();

    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(ingredient => ingredient.Name).HasMaxLength(200);
            entity.Property(ingredient => ingredient.Unit).HasMaxLength(30);
            entity.Property(ingredient => ingredient.CostPerUnit)
                .HasPrecision(12, 4);
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.Property(recipe => recipe.YieldQuantity).HasPrecision(12, 3);

            entity.HasOne(recipe => recipe.Product)
                .WithOne(product => product.Recipe)
                .HasForeignKey<Recipe>(recipe => recipe.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.Property(recipeIngredient => recipeIngredient.Quantity)
                .HasPrecision(12, 3);

            entity.HasIndex(recipeIngredient => new
            {
                recipeIngredient.RecipeId,
                recipeIngredient.IngredientId
            }).IsUnique();

            entity.HasOne(recipeIngredient => recipeIngredient.Recipe)
                .WithMany(recipe => recipe.RecipeIngredients)
                .HasForeignKey(recipeIngredient => recipeIngredient.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(recipeIngredient => recipeIngredient.Ingredient)
                .WithMany(ingredient => ingredient.RecipeIngredients)
                .HasForeignKey(recipeIngredient => recipeIngredient.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.Property(sale => sale.Quantity).HasPrecision(12, 3);
            entity.Property(sale => sale.UnitPrice).HasPrecision(12, 2);
            entity.HasIndex(sale => new { sale.SaleDate, sale.ProductId });
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(supplier => supplier.Name).HasMaxLength(255).IsRequired();
            entity.Property(supplier => supplier.Phone).HasMaxLength(20);
            entity.Property(supplier => supplier.Email).HasMaxLength(255);
            entity.Property(supplier => supplier.Address).HasMaxLength(500);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.Property(purchase => purchase.TotalAmount).HasPrecision(14, 2);
            entity.Property(purchase => purchase.Status).HasMaxLength(50).IsRequired();
            entity.Property(purchase => purchase.InvoiceNumber).HasMaxLength(100);

            entity.HasOne(purchase => purchase.Supplier)
                .WithMany(supplier => supplier.Purchases)
                .HasForeignKey(purchase => purchase.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(purchase => purchase.PurchaseDate);
        });

        modelBuilder.Entity<PurchaseItem>(entity =>
        {
            entity.Property(item => item.Quantity).HasPrecision(12, 3);
            entity.Property(item => item.UnitCost).HasPrecision(12, 4);
            entity.Property(item => item.TotalCost).HasPrecision(14, 2);

            entity.HasOne(item => item.Purchase)
                .WithMany(purchase => purchase.Items)
                .HasForeignKey(item => item.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(item => item.Ingredient)
                .WithMany()
                .HasForeignKey(item => item.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventoryStock>(entity =>
        {
            entity.Property(stock => stock.Quantity).HasPrecision(12, 3);
            entity.Property(stock => stock.ReorderLevel).HasPrecision(12, 3);

            entity.HasOne(stock => stock.Ingredient)
                .WithOne()
                .HasForeignKey<InventoryStock>(stock => stock.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(stock => stock.IngredientId).IsUnique();
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.Property(trans => trans.Quantity).HasPrecision(12, 3);
            entity.Property(trans => trans.TransactionType).HasMaxLength(50).IsRequired();

            entity.HasOne(trans => trans.Ingredient)
                .WithMany()
                .HasForeignKey(trans => trans.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(trans => new { trans.IngredientId, trans.CreatedAt });
        });
    }
}
