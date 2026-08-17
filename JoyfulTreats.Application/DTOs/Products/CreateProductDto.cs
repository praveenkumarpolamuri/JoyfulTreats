namespace JoyfulTreats.Application.DTOs.Products;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;

    public string? SKU { get; set; }

    public int CategoryId { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal MRP { get; set; }
}