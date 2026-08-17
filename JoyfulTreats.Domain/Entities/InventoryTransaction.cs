namespace JoyfulTreats.Domain.Entities;

public class InventoryTransaction
{
    public int Id { get; set; }

    public int IngredientId { get; set; }

    public string TransactionType { get; set; } = string.Empty; // PURCHASE, PRODUCTION, ADJUSTMENT, WASTAGE

    public decimal Quantity { get; set; }

    public int? ReferenceId { get; set; } // Purchase ID, Production Batch ID, etc.

    public DateTime CreatedAt { get; set; }

    public Ingredient Ingredient { get; set; } = null!;
}
