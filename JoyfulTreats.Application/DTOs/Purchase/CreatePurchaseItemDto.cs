namespace JoyfulTreats.Application.DTOs.Purchases;

public class CreatePurchaseItemDto
{
    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitCost { get; set; }
}