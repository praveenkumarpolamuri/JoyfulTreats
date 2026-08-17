import type { Category } from "../types/category";

const API_URL = "http://localhost:5020/api";

export async function getCategories(): Promise<Category[]> {
  const response = await fetch(`${API_URL}/Categories`);

  if (!response.ok) {
    throw new Error("Failed to fetch categories");
  }

  return response.json();
}