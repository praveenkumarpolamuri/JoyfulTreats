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
    public DbSet<IngredientPriceHistory> IngredientPriceHistories => Set<IngredientPriceHistory>(); // Added missing DbSet
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
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();
    public DbSet<ProductInventoryTransaction> ProductInventoryTransactions => Set<ProductInventoryTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ingredient
        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasIndex(e => e.Id);

            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Unit)
                .HasMaxLength(30)
                .IsRequired();
        });

        // Ingredient Price History
        modelBuilder.Entity<IngredientPriceHistory>(entity =>
        {
            entity.HasKey(ph => ph.Id);

            entity.Property(ph => ph.UnitCost)
                .HasPrecision(18, 4);

            entity.HasOne(ph => ph.Ingredient)
                .WithMany(i => i.PriceHistories)
                .HasForeignKey(ph => ph.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Recipe Ingredient
        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasIndex(e => new { e.RecipeId, e.IngredientId })
                .IsUnique();

            entity.Property(e => e.Quantity)
                .HasPrecision(12, 3);

            entity.Property(e => e.Unit)
                .IsRequired();

            entity.HasOne(e => e.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Production Batch
        modelBuilder.Entity<ProductionBatch>(entity =>
        {
            entity.Property(p => p.SnapshotUnitCost)
                .HasPrecision(18, 4);
        });

        // Sale
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasIndex(e => new { e.SaleDate, e.ProductId });

            entity.Property(e => e.Quantity)
                .HasPrecision(12, 3);

            entity.Property(e => e.UnitPrice)
                .HasPrecision(12, 2);
        });

        // Purchase
        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasIndex(e => e.PurchaseDate);

            entity.Property(e => e.TotalAmount)
                .HasPrecision(14, 2);

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(100);

            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.Purchases)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Purchase Item
        modelBuilder.Entity<PurchaseItem>(entity =>
        {
            entity.HasIndex(e => e.IngredientId);

            entity.Property(e => e.Quantity)
                .HasPrecision(12, 3);

            entity.Property(e => e.UnitCost)
                .HasPrecision(12, 4);

            entity.Property(e => e.TotalCost)
                .HasPrecision(14, 2);

            entity.HasOne(e => e.Ingredient)
                .WithMany()
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Purchase)
                .WithMany(p => p.Items)
                .HasForeignKey(e => e.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Inventory Stock
        modelBuilder.Entity<InventoryStock>(entity =>
        {
            entity.HasIndex(e => e.IngredientId)
                .IsUnique();

            entity.Property(e => e.Quantity)
                .HasPrecision(12, 3);

            entity.Property(e => e.ReorderLevel)
                .HasPrecision(12, 3);

            entity.HasOne(e => e.Ingredient)
                .WithMany()
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Inventory Transaction
        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasIndex(e => new { e.IngredientId, e.CreatedAt });

            entity.Property(e => e.Quantity)
                .HasPrecision(12, 3);

            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(e => e.Ingredient)
                .WithMany()
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Product Stock
        modelBuilder.Entity<ProductStock>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.ProductId)
                .IsUnique();

            entity.Property(e => e.Quantity)
                .HasPrecision(18, 3);

            entity.Property(e => e.ReorderLevel)
                .HasPrecision(18, 3);

            entity.HasOne(e => e.Product)
                .WithOne(p => p.ProductStock)
                .HasForeignKey<ProductStock>(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Product Inventory Transaction
        modelBuilder.Entity<ProductInventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Quantity)
                .HasPrecision(18, 3);

            entity.Property(e => e.TransactionType)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasOne(e => e.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}