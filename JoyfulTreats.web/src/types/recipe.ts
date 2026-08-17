export interface RecipeIngredient {
  ingredientId: number;
  ingredientName: string;
  unit: string;
  quantity: number;
  cost: number;
}

export interface Recipe {
  id: number;
  productId: number;
  productName: string;
  yieldQuantity: number;
  totalCost: number;
  costPerItem: number;
  ingredients: RecipeIngredient[];
}
