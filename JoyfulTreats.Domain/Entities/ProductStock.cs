namespace JoyfulTreats.Domain.Entities;

public class ProductStock
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal ReorderLevel { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
}