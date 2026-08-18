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

    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();

public DbSet<ProductInventoryTransaction> ProductInventoryTransactions
    => Set<ProductInventoryTransaction>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

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
