
namespace JoyfulTreats.Domain.Entities;

public class RecipeIngredient
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    // Unit used in this recipe line. It may differ from the ingredient purchase unit.
    public string Unit { get; set; } = string.Empty;

    public Recipe Recipe { get; set; } = null!;

    public Ingredient Ingredient { get; set; } = null!;
}
