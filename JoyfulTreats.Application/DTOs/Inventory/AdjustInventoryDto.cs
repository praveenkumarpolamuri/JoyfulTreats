namespace JoyfulTreats.Application.DTOs.Inventory;

public class AdjustInventoryDto
{
    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public string TransactionType { get; set; } = "ADJUSTMENT"; // ADJUSTMENT, WASTAGE
}
