// Domain/Entities/Ingredient.cs
public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty; // e.g., "g", "kg", "ml"

    // Navigation property for historical prices
    public ICollection<IngredientPriceHistory> PriceHistories { get; set; } 
        = new List<IngredientPriceHistory>();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

