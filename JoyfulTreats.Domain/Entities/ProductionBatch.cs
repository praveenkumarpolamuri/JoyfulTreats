
// Domain/Entities/ProductionBatch.cs
using JoyfulTreats.Domain.Entities;

public class ProductionBatch
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal BatchQuantity { get; set; }
    
    // Crucial: Snapshot of total unit cost at the time of production
    public decimal SnapshotUnitCost { get; set; } 
    public DateTime ProducedAt { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
}