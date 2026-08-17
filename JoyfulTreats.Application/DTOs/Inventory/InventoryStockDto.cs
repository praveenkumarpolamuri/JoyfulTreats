namespace JoyfulTreats.Application.DTOs.Inventory;

public class InventoryStockDto
{
    public int Id { get; set; }

    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal ReorderLevel { get; set; }

    public bool IsLowStock => Quantity <= ReorderLevel;
}
