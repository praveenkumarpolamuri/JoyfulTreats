namespace JoyfulTreats.Domain.Entities;

public class Recipe
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    // The number of finished items made by one batch of this recipe.
    public decimal YieldQuantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        = new List<RecipeIngredient>();
}
