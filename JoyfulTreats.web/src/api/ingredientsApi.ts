import type { Ingredient } from "../types/ingredient";

const API_URL = "http://localhost:5020/api";

export async function getIngredients(): Promise<Ingredient[]> {
  const response = await fetch(`${API_URL}/Ingredients`);

  if (!response.ok) {
    throw new Error("Failed to fetch ingredients");
  }

  return response.json();
}

export interface CreateIngredientRequest {
  name: string;
  unit: string;
  costPerUnit: number;
}

export async function createIngredient(
  ingredient: CreateIngredientRequest,
): Promise<Ingredient> {
  const response = await fetch(`${API_URL}/Ingredients`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(ingredient),
  });

  if (!response.ok) {
    throw new Error("Failed to create ingredient");
  }

  return response.json();
}

export async function getIngredient(id: number): Promise<Ingredient> {
  const response = await fetch(`${API_URL}/Ingredients/${id}`);
  if (!response.ok) throw new Error("Failed to fetch ingredient");
  return response.json();
}

export async function updateIngredient(id: number, ingredient: CreateIngredientRequest): Promise<Ingredient> {
  const response = await fetch(`${API_URL}/Ingredients/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ ...ingredient, isActive: true }),
  });
  if (!response.ok) throw new Error("Failed to update ingredient");
  return response.json();
}

export async function deleteIngredient(id: number): Promise<void> {
  const response = await fetch(`${API_URL}/Ingredients/${id}`, { method: "DELETE" });
  if (!response.ok) throw new Error("Failed to delete ingredient");
}
