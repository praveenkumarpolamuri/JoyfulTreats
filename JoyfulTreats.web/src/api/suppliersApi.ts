


const API_URL = "http://localhost:5020/api";

export async function getSuppliers(): Promise<Supplier[]> {
  const response = await fetch(`${API_URL}/Suppliers`);

  if (!response.ok) {
    throw new Error("Failed to fetch suppliers");
  }

  return response.json();
}

export async function getSupplier(id: number): Promise<Supplier> {
  const response = await fetch(`${API_URL}/Suppliers/${id}`);

  if (!response.ok) {
    throw new Error("Failed to fetch supplier");
  }

  return response.json();
}
export interface Supplier {
  id: number;
  name: string;
  phone?: string | null;
  email?: string | null;
  address?: string | null;
  isActive: boolean;
}

export interface CreateSupplierRequest {
  name: string;
  phone?: string;
  email?: string;
  address?: string;
}

export async function createSupplier(
  supplier: CreateSupplierRequest,
): Promise<Supplier> {
  const response = await fetch(`${API_URL}/Suppliers`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(supplier),
  });

  if (!response.ok) {
    throw new Error("Failed to create supplier");
  }

  return response.json();
}

export async function updateSupplier(
  id: number,
  supplier: CreateSupplierRequest,
): Promise<Supplier> {
  const response = await fetch(`${API_URL}/Suppliers/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      ...supplier,
      isActive: true,
    }),
  });

  if (!response.ok) {
    throw new Error("Failed to update supplier");
  }

  return response.json();
}

export async function deleteSupplier(id: number): Promise<void> {
  const response = await fetch(`${API_URL}/Suppliers/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    throw new Error("Failed to delete supplier");
  }
}