namespace JoyfulTreats.Application.DTOs.Purchases;

public class PurchaseItemDto
{
    public int Id { get; set; }

    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal TotalCost { get; set; }
}