namespace JoyfulTreats.Domain.Entities;

public class Sale
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public DateOnly SaleDate { get; set; }

    public decimal Quantity { get; set; }

    // Captured at the time of sale so later product price changes do not alter history.
    public decimal UnitPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
}
