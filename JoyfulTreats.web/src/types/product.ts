export interface Product {
  id: number;
  name: string;
  sku: string | null;
  categoryId: number;
  categoryName: string;
  sellingPrice: number;
  mrp: number;
  isActive: boolean;
}