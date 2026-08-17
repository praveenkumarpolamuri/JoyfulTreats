namespace JoyfulTreats.Application.DTOs.Recipes;

public class RecipeDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal YieldQuantity { get; set; }

    public decimal TotalCost { get; set; }

    public decimal CostPerItem { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal GrossProfit { get; set; }

    public decimal MarginPercentage { get; set; }

    public IReadOnlyList<RecipeIngredientDto> Ingredients { get; set; }
        = [];
}
