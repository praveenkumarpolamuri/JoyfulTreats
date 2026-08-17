namespace JoyfulTreats.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateOnly OrderDate { get; set; }

    public string Status { get; set; } = "PENDING"; // PENDING, CONFIRMED, IN_PRODUCTION, READY, DELIVERED, CANCELLED

    public decimal Subtotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentStatus { get; set; } = "UNPAID"; // UNPAID, PAID, REFUNDED

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Customer Customer { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
