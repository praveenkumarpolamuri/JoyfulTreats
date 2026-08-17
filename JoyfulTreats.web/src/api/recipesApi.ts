import type { Recipe } from "../types/recipe";

const API_URL = "http://localhost:5020/api";

async function getErrorMessage(response: Response, fallback: string): Promise<string> {
  try {
    const body = await response.json() as { detail?: string; title?: string };
    return body.detail ?? body.title ?? fallback;
  } catch {
    return fallback;
  }
}

export interface CreateRecipeRequest {
  productId: number;
  yieldQuantity: number;
  ingredients: Array<{
    ingredientId: number;
    quantity: number;
    unit: string;
  }>;
}

export async function getRecipes(): Promise<Recipe[]> {
  const response = await fetch(`${API_URL}/Recipes`);

  if (!response.ok) {
    throw new Error(await getErrorMessage(response, "Failed to fetch recipes"));
  }

  return response.json();
}

export async function createRecipe(
  recipe: CreateRecipeRequest,
): Promise<Recipe> {
  const response = await fetch(`${API_URL}/Recipes`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(recipe),
  });

  if (!response.ok) throw new Error(await getErrorMessage(response, "Failed to create recipe"));

  return response.json();
}

export async function getRecipe(id: number): Promise<Recipe> {
  const response = await fetch(`${API_URL}/Recipes/${id}`);
  if (!response.ok) throw new Error(await getErrorMessage(response, "Failed to fetch recipe"));
  return response.json();
}

export async function updateRecipe(id: number, recipe: CreateRecipeRequest): Promise<Recipe> {
  const response = await fetch(`${API_URL}/Recipes/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(recipe),
  });
  if (!response.ok) throw new Error(await getErrorMessage(response, "Failed to update recipe"));
  return response.json();
}

export async function deleteRecipe(id: number): Promise<void> {
  const response = await fetch(`${API_URL}/Recipes/${id}`, { method: "DELETE" });
  if (!response.ok) throw new Error(await getErrorMessage(response, "Failed to delete recipe"));
}
