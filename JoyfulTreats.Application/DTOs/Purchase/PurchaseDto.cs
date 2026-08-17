namespace JoyfulTreats.Application.DTOs.Purchases;

public class PurchaseDto
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public DateOnly PurchaseDate { get; set; }

    public string? InvoiceNumber { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<PurchaseItemDto> Items { get; set; } = new();
}