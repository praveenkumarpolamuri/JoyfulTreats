namespace JoyfulTreats.Application.DTOs.Purchases;

public class CreatePurchaseDto
{
    public int SupplierId { get; set; }

    public DateOnly PurchaseDate { get; set; }

    public string? InvoiceNumber { get; set; }

    public List<CreatePurchaseItemDto> Items { get; set; } = new();
}