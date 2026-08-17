import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  createProduct,
  type CreateProductRequest,
} from "../api/productsApi";
import { getCategories } from "../api/categoriesApi";
import type { Category } from "../types/category";

function AddProductPage() {
  const navigate = useNavigate();

  const [form, setForm] = useState<CreateProductRequest>({
    name: "",
    sku: "",
    categoryId: 1,
    sellingPrice: 0,
    mrp: 0,
  });
const [categories, setCategories] = useState<Category[]>([]);
const [loadingCategories, setLoadingCategories] = useState(true);

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleChange(
  event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
) {
  const { name, value } = event.target;

  setForm((current) => ({
    ...current,
    [name]:
      name === "categoryId" ||
      name === "sellingPrice" ||
      name === "mrp"
        ? Number(value)
        : value,
  }));
}


  useEffect(() => {
  async function loadCategories() {
    try {
      const data = await getCategories();
      setCategories(data);
    } catch {
      setError("Unable to load categories.");
    } finally {
      setLoadingCategories(false);
    }
  }

  loadCategories();
}, []);

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    setSaving(true);
    setError(null);

    try {
      await createProduct(form);
      navigate("/products");
    } catch {
      setError("Unable to create product.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div>
      <h1>Add Product</h1>

      {error && <p>{error}</p>}

      <form onSubmit={handleSubmit}>
        <div>
          <label>Product Name</label>
          <br />
          <input
            name="name"
            value={form.name}
            onChange={handleChange}
            required
          />
        </div>

        <br />

        <div>
          <label>SKU</label>
          <br />
          <input
            name="sku"
            value={form.sku}
            onChange={handleChange}
          />
        </div>

        <br />

        <div>
  <label>Category</label>
  <br />

  <select
    name="categoryId"
    value={form.categoryId}
    onChange={handleChange}
    disabled={loadingCategories}
    required  >
    <option value="">
      {loadingCategories ? "Loading categories..." : "Select category"}
    </option>

    {categories.map((category) => (
      <option key={category.id} value={category.id}>
        {category.name}
      </option>
    ))}
  </select>
</div>

        <br />

        <div>
          <label>Selling Price</label>
          <br />
          <input
            type="number"
            name="sellingPrice"
            value={form.sellingPrice}
            onChange={handleChange}
            required
          />
        </div>

        <br />

        <div>
          <label>MRP</label>
          <br />
          <input
            type="number"
            name="mrp"
            value={form.mrp}
            onChange={handleChange}
            required
          />
        </div>

        <br />

        <button type="submit" disabled={saving}>
          {saving ? "Saving..." : "Save Product"}
        </button>

        {" "}

        <button
          type="button"
          onClick={() => navigate("/products")}
        >
          Cancel
        </button>
      </form>
    </div>
  );
}

export default AddProductPage;