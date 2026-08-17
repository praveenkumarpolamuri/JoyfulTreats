import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getProducts } from "../api/productsApi";
import { createSale, getSale, updateSale, type SaveSaleRequest } from "../api/salesApi";
import type { Product } from "../types/product";

function today() { return new Date().toISOString().slice(0, 10); }

function SaveSalePage() {
  const { id } = useParams<{ id: string }>();
  const isEditing = Boolean(id);
  const navigate = useNavigate();
  const [products, setProducts] = useState<Product[]>([]);
  const [form, setForm] = useState<SaveSaleRequest>({ productId: 0, saleDate: today(), quantity: 1, unitPrice: 0 });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadForm() {
      try {
        const [productsData, sale] = await Promise.all([getProducts(), id ? getSale(Number(id)) : Promise.resolve(null)]);
        setProducts(productsData);
        if (sale) setForm({ productId: sale.productId, saleDate: sale.saleDate, quantity: sale.quantity, unitPrice: sale.unitPrice });
      } catch (err) {
        setError(err instanceof Error ? err.message : "Unable to load sale.");
      } finally { setLoading(false); }
    }
    loadForm();
  }, [id]);

  function selectProduct(productId: number) {
    const product = products.find((item) => item.id === productId);
    setForm((current) => ({ ...current, productId, unitPrice: product?.sellingPrice ?? current.unitPrice }));
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true); setError(null);
    try {
      if (id) await updateSale(Number(id), form); else await createSale(form);
      navigate("/sales");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to save sale.");
    } finally { setSaving(false); }
  }

  if (loading) return <p>Loading sale form...</p>;
  return <div>
    <h1>{isEditing ? "Edit Sale" : "Add Sale"}</h1>
    {error && <p>{error}</p>}
    <form onSubmit={handleSubmit}>
      <div><label htmlFor="saleDate">Sale Date</label><br /><input id="saleDate" type="date" value={form.saleDate} onChange={(event) => setForm((current) => ({ ...current, saleDate: event.target.value }))} required /></div><br />
      <div><label htmlFor="product">Product</label><br /><select id="product" value={form.productId} onChange={(event) => selectProduct(Number(event.target.value))} required><option value={0}>Select product</option>{products.map((product) => <option key={product.id} value={product.id}>{product.name}</option>)}</select></div><br />
      <div><label htmlFor="quantity">Quantity Sold</label><br /><input id="quantity" type="number" min="0.001" step="0.001" value={form.quantity} onChange={(event) => setForm((current) => ({ ...current, quantity: Number(event.target.value) }))} required /></div><br />
      <div><label htmlFor="unitPrice">Sale Price per Item</label><br /><input id="unitPrice" type="number" min="0" step="0.01" value={form.unitPrice} onChange={(event) => setForm((current) => ({ ...current, unitPrice: Number(event.target.value) }))} required /></div><br />
      <p>Total: ₹{(form.quantity * form.unitPrice).toFixed(2)}</p>
      <button type="submit" disabled={saving}>{saving ? "Saving..." : isEditing ? "Update Sale" : "Save Sale"}</button>{" "}
      <button type="button" onClick={() => navigate("/sales")}>Cancel</button>
    </form>
  </div>;
}

export default SaveSalePage;
