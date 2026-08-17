export const unitOptions = ["g", "kg", "ml", "l", "piece"] as const;

type Unit = (typeof unitOptions)[number];

const unitDetails: Record<Unit, { dimension: string; baseFactor: number }> = {
  g: { dimension: "weight", baseFactor: 1 },
  kg: { dimension: "weight", baseFactor: 1000 },
  ml: { dimension: "volume", baseFactor: 1 },
  l: { dimension: "volume", baseFactor: 1000 },
  piece: { dimension: "count", baseFactor: 1 },
};

export function compatibleUnits(unit: string): Unit[] {
  const details = unitDetails[unit as Unit];
  return details
    ? unitOptions.filter((option) => unitDetails[option].dimension === details.dimension)
    : [];
}

export function calculateRecipeLineCost(
  quantity: number,
  recipeUnit: string,
  purchaseUnit: string,
  purchaseCostPerUnit: number,
): number {
  const from = unitDetails[recipeUnit as Unit];
  const to = unitDetails[purchaseUnit as Unit];
  if (!from || !to || from.dimension !== to.dimension) return 0;

  return (quantity * from.baseFactor / to.baseFactor) * purchaseCostPerUnit;
}
