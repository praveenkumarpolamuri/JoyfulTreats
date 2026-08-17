namespace JoyfulTreats.Application.DTOs.Ingredients;

public class UpdateIngredientDto
{
    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public decimal CostPerUnit { get; set; }

    public bool IsActive { get; set; }
}
