namespace JoyfulTreats.Domain.Entities;

public class ProductionBatch
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int RecipeId { get; set; }

    public decimal PlannedQuantity { get; set; }

    public decimal ProducedQuantity { get; set; }

    public DateOnly ProductionDate { get; set; }

    public string Status { get; set; } = "PLANNED"; // PLANNED, IN_PROGRESS, COMPLETED, CANCELLED

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;

    public Recipe Recipe { get; set; } = null!;
}
