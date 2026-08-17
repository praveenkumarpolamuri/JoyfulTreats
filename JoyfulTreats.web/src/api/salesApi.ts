import type { Sale } from "../types/sale";

const API_URL = "http://localhost:5020/api";

export interface SaveSaleRequest {
  productId: number;
  saleDate: string;
  quantity: number;
  unitPrice: number;
}

async function ensureSuccess(response: Response, fallback: string) {
  if (response.ok) return;
  try {
    const error = await response.json() as { detail?: string; title?: string };
    throw new Error(error.detail ?? error.title ?? fallback);
  } catch (error) {
    throw error instanceof Error ? error : new Error(fallback);
  }
}

export async function getSales(): Promise<Sale[]> {
  const response = await fetch(`${API_URL}/Sales`);
  await ensureSuccess(response, "Failed to fetch sales");
  return response.json();
}

export async function getSale(id: number): Promise<Sale> {
  const response = await fetch(`${API_URL}/Sales/${id}`);
  await ensureSuccess(response, "Failed to fetch sale");
  return response.json();
}

export async function createSale(sale: SaveSaleRequest): Promise<Sale> {
  const response = await fetch(`${API_URL}/Sales`, {
    method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(sale),
  });
  await ensureSuccess(response, "Failed to create sale");
  return response.json();
}

export async function updateSale(id: number, sale: SaveSaleRequest): Promise<Sale> {
  const response = await fetch(`${API_URL}/Sales/${id}`, {
    method: "PUT", headers: { "Content-Type": "application/json" }, body: JSON.stringify(sale),
  });
  await ensureSuccess(response, "Failed to update sale");
  return response.json();
}

export async function deleteSale(id: number): Promise<void> {
  const response = await fetch(`${API_URL}/Sales/${id}`, { method: "DELETE" });
  await ensureSuccess(response, "Failed to delete sale");
}
