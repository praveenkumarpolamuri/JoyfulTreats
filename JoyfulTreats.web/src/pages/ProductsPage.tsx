import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  deleteProduct,
  getProducts,
} from "../api/productsApi";
import type { Product } from "../types/product";


function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadProducts() {
      try {
        const data = await getProducts();
        setProducts(data);
      } catch {
        setError("Unable to load products.");
      } finally {
        setLoading(false);
      }
    }

    loadProducts();
  }, []);

async function handleDelete(id: number, name: string) {
  const confirmed = window.confirm(
    `Are you sure you want to deactivate "${name}"?`
  );

  if (!confirmed) {
    return;
  }

  try {
    await deleteProduct(id);

    setProducts((current) =>
      current.filter((product) => product.id !== id)
    );
  } catch {
    setError("Unable to delete product.");
  }
}

  if (loading) {
    return <p>Loading products...</p>;
  }

  if (error) {
    return <p>{error}</p>;
  }

  return (
    <div>
      <div>
        <h1>Products</h1>

        <Link to="/products/new">
          <button>+ Add Product</button>
        </Link>
      </div>

      {products.length === 0 ? (
        <p>No products found.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Product</th>
              <th>Category</th>
              <th>SKU</th>
              <th>Price</th>
              <th>MRP</th>
              <th>Actions</th>
            </tr>
          </thead>

          <tbody>
            {products.map((product) => (
              <tr key={product.id}>
                <td>{product.name}</td>
                <td>{product.categoryName}</td>
                <td>{product.sku ?? "-"}</td>
                <td>₹{product.sellingPrice}</td>
                <td>₹{product.mrp}</td>
                <td>
  <Link to={`/products/${product.id}/edit`}>
    Edit
  </Link>

  {" "}

  <button
    onClick={() => handleDelete(product.id, product.name)}
  >
    Delete
  </button>
</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}





export default ProductsPage;