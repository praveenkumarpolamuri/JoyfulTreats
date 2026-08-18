// Domain/Entities/IngredientPriceHistory.cs
public class IngredientPriceHistory
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; } // Null means current active price

    public Ingredient? Ingredient { get; set; }
}
