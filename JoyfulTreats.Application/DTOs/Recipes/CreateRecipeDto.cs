namespace JoyfulTreats.Application.DTOs.Recipes;

public class CreateRecipeDto
{
    public int ProductId { get; set; }

    public decimal YieldQuantity { get; set; }

    public IReadOnlyList<CreateRecipeIngredientDto> Ingredients { get; set; } = [];
}

public class CreateRecipeIngredientDto
{
    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;
}
