
const API_URL = "http://localhost:5020/api";

export interface PurchaseItem {
  id: number;
  ingredientId: number;
  ingredientName: string;
  unit: string;
  quantity: number;
  unitCost: number;
  totalCost: number;
}

export interface Purchase {
  id: number;
  supplierId: number;
  supplierName: string;
  purchaseDate: string;
  invoiceNumber?: string;
  totalAmount: number;
  status: string;
  items: PurchaseItem[];
}

export interface CreatePurchaseItemRequest {
  ingredientId: number;
  quantity: number;
  unitCost: number;
}

export interface CreatePurchaseRequest {
  supplierId: number;
  purchaseDate: string;
  invoiceNumber?: string;
  items: CreatePurchaseItemRequest[];
}

export async function getPurchases(): Promise<Purchase[]> {
  const response = await fetch(`${API_URL}/Purchases`);

  if (!response.ok) {
    throw new Error("Failed to fetch purchases");
  }

  return response.json();
}

export async function getPurchase(id: number): Promise<Purchase> {
  const response = await fetch(`${API_URL}/Purchases/${id}`);

  if (!response.ok) {
    throw new Error("Failed to fetch purchase");
  }

  return response.json();
}

export async function createPurchase(
  purchase: CreatePurchaseRequest
): Promise<Purchase> {
  const response = await fetch(`${API_URL}/Purchases`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(purchase),
  });

  if (!response.ok) {
    throw new Error("Failed to create purchase");
  }

  return response.json();
}

export async function updatePurchase(
  id: number,
  purchase: CreatePurchaseRequest
): Promise<Purchase> {
  const response = await fetch(`${API_URL}/Purchases/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(purchase),
  });

  if (!response.ok) {
    throw new Error("Failed to update purchase");
  }

  return response.json();
}

export async function receivePurchase(id: number): Promise<Purchase> {
  const response = await fetch(`${API_URL}/Purchases/${id}/receive`, {
    method: "POST",
  });

  if (!response.ok) {
    throw new Error("Failed to receive purchase");
  }

  return response.json();
}

export async function cancelPurchase(id: number): Promise<void> {
  const response = await fetch(`${API_URL}/Purchases/${id}/cancel`, {
    method: "POST",
  });

  if (!response.ok) {
    throw new Error("Failed to cancel purchase");
  }
}