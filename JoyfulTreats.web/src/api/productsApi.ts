import type { Product } from "../types/product";

const API_URL = "http://localhost:5020/api";

export interface CreateProductRequest {
  name: string;
  sku: string;
  categoryId: number;
  sellingPrice: number;
  mrp: number;
}

export async function getProducts(): Promise<Product[]> {
  const response = await fetch(`${API_URL}/Products`);

  if (!response.ok) {
    throw new Error("Failed to fetch products");
  }

  return response.json();
}

export async function getProduct(id: number): Promise<Product> {
  const response = await fetch(`${API_URL}/Products/${id}`);

  if (!response.ok) {
    throw new Error("Failed to fetch product");
  }

  return response.json();
}

export async function createProduct(
  product: CreateProductRequest
): Promise<Product> {
  const response = await fetch(`${API_URL}/Products`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(product),
  });

  if (!response.ok) {
    throw new Error("Failed to create product");
  }

  return response.json();
}

export async function updateProduct(
  id: number,
  product: CreateProductRequest
): Promise<Product> {
  const response = await fetch(`${API_URL}/Products/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(product),
  });

  if (!response.ok) {
    throw new Error("Failed to update product");
  }

  return response.json();
}

export async function deleteProduct(id: number): Promise<void> {
  const response = await fetch(`${API_URL}/Products/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    throw new Error("Failed to delete product");
  }
}