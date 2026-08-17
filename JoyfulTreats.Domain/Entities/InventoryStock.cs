namespace JoyfulTreats.Domain.Entities;

public class InventoryStock
{
    public int Id { get; set; }

    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public decimal ReorderLevel { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Ingredient Ingredient { get; set; } = null!;
}
