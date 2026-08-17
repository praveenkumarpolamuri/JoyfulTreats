import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  getProduct,
  updateProduct,
  type CreateProductRequest,
} from "../api/productsApi";
import { getCategories } from "../api/categoriesApi";
import type { Category } from "../types/category";

function EditProductPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [categories, setCategories] = useState<Category[]>([]);

  const [form, setForm] = useState<CreateProductRequest>({
    name: "",
    sku: "",
    categoryId: 0,
    sellingPrice: 0,
    mrp: 0,
  });

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadData() {
      try {
        const [product, categories] = await Promise.all([
          getProduct(Number(id)),
          getCategories(),
        ]);

        setForm({
          name: product.name,
          sku: product.sku ?? "",
          categoryId: product.categoryId,
          sellingPrice: product.sellingPrice,
          mrp: product.mrp,
        });

        setCategories(categories);
      } catch (err) {
  console.error("Unable to load product:", err);

  if (err instanceof Error) {
    setError(err.message);
  } else {
    setError("Unable to load product.");
  }
} finally {
        setLoading(false);
      }
    }

    loadData();
  }, [id]);

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

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!id) {
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await updateProduct(Number(id), form);
      navigate("/products");
    } catch {
      setError("Unable to update product.");
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return <p>Loading product...</p>;
  }

  if (error) {
    return <p>{error}</p>;
  }

  return (
    <div>
      <h1>Edit Product</h1>

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
            required
          >
            <option value="">Select category</option>

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
          {saving ? "Saving..." : "Update Product"}
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

export default EditProductPage;