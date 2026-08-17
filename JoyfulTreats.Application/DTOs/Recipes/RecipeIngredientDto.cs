namespace JoyfulTreats.Application.DTOs.Recipes;

public class RecipeIngredientDto
{
    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal Cost { get; set; }
}
