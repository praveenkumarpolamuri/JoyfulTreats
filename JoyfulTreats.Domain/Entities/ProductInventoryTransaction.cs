namespace JoyfulTreats.Domain.Entities;

public class ProductInventoryTransaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string TransactionType { get; set; } = string.Empty;
    // PRODUCTION, SALE, ADJUSTMENT, WASTAGE

    public decimal Quantity { get; set; }

    public int? ReferenceId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; } = null!;
}