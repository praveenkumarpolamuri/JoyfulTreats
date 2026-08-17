namespace JoyfulTreats.Application.DTOs.Sales;

public class CreateSaleDto
{
    public int ProductId { get; set; }

    public DateOnly SaleDate { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
