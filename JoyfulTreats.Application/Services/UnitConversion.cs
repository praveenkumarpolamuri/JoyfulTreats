namespace JoyfulTreats.Application.Services;

public static class UnitConversion
{
    private static readonly IReadOnlyDictionary<string, (string Dimension, decimal BaseFactor)> Units =
        new Dictionary<string, (string, decimal)>(StringComparer.OrdinalIgnoreCase)
        {
            ["g"] = ("weight", 1m),
            ["kg"] = ("weight", 1000m),
            ["ml"] = ("volume", 1m),
            ["l"] = ("volume", 1000m),
            ["piece"] = ("count", 1m)
        };

    public static bool IsSupported(string unit) => Units.ContainsKey(unit.Trim());

    public static decimal Convert(decimal quantity, string fromUnit, string toUnit)
    {
        var from = GetUnit(fromUnit);
        var to = GetUnit(toUnit);

        if (from.Dimension != to.Dimension)
            throw new ArgumentException($"Cannot convert {fromUnit} to {toUnit}.");

        return quantity * from.BaseFactor / to.BaseFactor;
    }

    public static decimal CalculateCost(
        decimal recipeQuantity,
        string recipeUnit,
        decimal purchaseCostPerUnit,
        string purchaseUnit) =>
        Convert(recipeQuantity, recipeUnit, purchaseUnit) * purchaseCostPerUnit;

    private static (string Dimension, decimal BaseFactor) GetUnit(string unit) =>
        Units.TryGetValue(unit.Trim(), out var value)
            ? value
            : throw new ArgumentException($"Unsupported unit: {unit}.");
}
