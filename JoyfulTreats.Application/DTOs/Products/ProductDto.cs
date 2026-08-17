namespace JoyfulTreats.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? SKU { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal SellingPrice { get; set; }

    public decimal MRP { get; set; }

    public bool IsActive { get; set; }
}