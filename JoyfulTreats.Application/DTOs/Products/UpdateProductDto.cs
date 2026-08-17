public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal MRP { get; set; }

    public bool IsActive { get; set; }
}