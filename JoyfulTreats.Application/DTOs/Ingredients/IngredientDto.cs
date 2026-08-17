namespace JoyfulTreats.Application.DTOs.Ingredients;

public class IngredientDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public decimal CostPerUnit { get; set; }

    public bool IsActive { get; set; }
}
