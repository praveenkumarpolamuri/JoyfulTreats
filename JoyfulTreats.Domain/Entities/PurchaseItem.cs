namespace JoyfulTreats.Domain.Entities;

public class PurchaseItem
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal TotalCost { get; set; }

    public Purchase Purchase { get; set; } = null!;

    public Ingredient Ingredient { get; set; } = null!;
}
