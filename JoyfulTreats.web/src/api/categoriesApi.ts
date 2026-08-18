import type { Category } from "../types/category";

const API_URL = "http://localhost:5020/api";

export async function getCategories(): Promise<Category[]> {
  const response = await fetch(`${API_URL}/Categories`);

  if (!response.ok) {
    throw new Error("Failed to fetch categories");
  }

  return response.json();
}


export async function createCategory(category: Category): Promise<Category> {
  const response = await fetch(`${API_URL}/Categories`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(category),
  });

  if (!response.ok) {
    throw new Error("Failed to create category");
  }

  return response.json();
}


export async function updateCategory(category: Category): Promise<Category> {
  const response = await fetch(`${API_URL}/Categories/${category.id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(category),
  });

  if (!response.ok) {
    throw new Error("Failed to update category");
  }

  return response.json();
} 


