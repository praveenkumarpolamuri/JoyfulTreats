namespace JoyfulTreats.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? SKU { get; set; }

    public int CategoryId { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal MRP { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;

    public Recipe? Recipe { get; set; }

    public ProductStock? ProductStock { get; set; }

public ICollection<ProductInventoryTransaction> InventoryTransactions { get; set; }
    = new List<ProductInventoryTransaction>();
}
