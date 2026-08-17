namespace JoyfulTreats.Domain.Entities;

public class Purchase
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public DateOnly PurchaseDate { get; set; }

    public string? InvoiceNumber { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "PENDING"; // PENDING, RECEIVED, CANCELLED

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Supplier Supplier { get; set; } = null!;

    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}
