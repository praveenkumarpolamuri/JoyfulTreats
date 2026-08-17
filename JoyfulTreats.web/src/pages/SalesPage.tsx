import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { deleteSale, getSales } from "../api/salesApi";
import type { Sale } from "../types/sale";

const currency = new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR" });

function SalesPage() {
  const [sales, setSales] = useState<Sale[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getSales().then(setSales).catch((err) => setError(err.message)).finally(() => setLoading(false));
  }, []);

  async function handleDelete(id: number, productName: string) {
    if (!window.confirm(`Delete the sale record for "${productName}"?`)) return;
    try {
      await deleteSale(id);
      setSales((current) => current.filter((sale) => sale.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to delete sale.");
    }
  }

  if (loading) return <p>Loading sales...</p>;

  return <div>
    <h1>Sales</h1>
    <Link to="/sales/new"><button>+ Add Sale</button></Link>
    {error && <p>{error}</p>}
    {!error && sales.length === 0 && <p>No sales recorded.</p>}
    {!error && sales.length > 0 && <table>
      <thead><tr><th>Date</th><th>Product</th><th>Quantity</th><th>Unit Price</th><th>Total</th><th>Actions</th></tr></thead>
      <tbody>{sales.map((sale) => <tr key={sale.id}>
        <td>{sale.saleDate}</td><td>{sale.productName}</td><td>{sale.quantity}</td>
        <td>{currency.format(sale.unitPrice)}</td><td>{currency.format(sale.totalAmount)}</td>
        <td><Link to={`/sales/${sale.id}/edit`}>Edit</Link>{" "}<button onClick={() => handleDelete(sale.id, sale.productName)}>Delete</button></td>
      </tr>)}</tbody>
    </table>}
  </div>;
}

export default SalesPage;
