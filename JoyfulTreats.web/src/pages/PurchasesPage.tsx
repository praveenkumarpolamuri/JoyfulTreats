import { useEffect, useState } from "react";
import {
  createPurchase,
  getPurchases,
  receivePurchase,
  cancelPurchase,
  type Purchase,
} from "../api/purchaseApi";

import { getSuppliers } from "../api/suppliersApi";
import { getIngredients } from "../api/ingredientsApi";
import type { Supplier } from "../types/supplier";
import type { Ingredient } from "../types/ingredient";


interface PurchaseFormItem {
  ingredientId: number;
  quantity: number;
  unitCost: number;
}

function PurchasesPage() {
  const [purchases, setPurchases] = useState<Purchase[]>([]);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);

  const [supplierId, setSupplierId] = useState<number>(0);

  const [purchaseDate, setPurchaseDate] = useState(
    new Date().toISOString().split("T")[0],
  );

  const [invoiceNumber, setInvoiceNumber] = useState("");

  const [items, setItems] = useState<PurchaseFormItem[]>([
    {
      ingredientId: 0,
      quantity: 0,
      unitCost: 0,
    },
  ]);

  async function loadPurchases() {
    try {
      setLoading(true);
      setError(null);

      const data = await getPurchases();
      setPurchases(data);
    } catch {
      setError("Unable to load purchases.");
    } finally {
      setLoading(false);
    }
  }

  async function loadFormData() {
    try {
      const [supplierData, ingredientData] = await Promise.all([
        getSuppliers(),
        getIngredients(),
      ]);

      setSuppliers(supplierData);
      setIngredients(ingredientData);
    } catch {
      setError("Unable to load suppliers or ingredients.");
    }
  }

  useEffect(() => {
    async function load() {
      await Promise.all([
        loadPurchases(),
        loadFormData(),
      ]);
    }

    load();
  }, []);

  function openNewPurchase() {
    setSupplierId(0);

    setPurchaseDate(
      new Date().toISOString().split("T")[0],
    );

    setInvoiceNumber("");

    setItems([
      {
        ingredientId: 0,
        quantity: 0,
        unitCost: 0,
      },
    ]);

    setError(null);
    setShowForm(true);
  }

  function closeForm() {
    setShowForm(false);
    setError(null);
  }

  function updateItem(
    index: number,
    field: keyof PurchaseFormItem,
    value: number,
  ) {
    setItems((current) =>
      current.map((item, itemIndex) =>
        itemIndex === index
          ? {
              ...item,
              [field]: value,
            }
          : item,
      ),
    );
  }

  function addItem() {
    setItems((current) => [
      ...current,
      {
        ingredientId: 0,
        quantity: 0,
        unitCost: 0,
      },
    ]);
  }

  function removeItem(index: number) {
    setItems((current) =>
      current.filter((_, itemIndex) => itemIndex !== index),
    );
  }

  function getItemTotal(item: PurchaseFormItem) {
    return item.quantity * item.unitCost;
  }

  const totalAmount = items.reduce(
    (total, item) => total + getItemTotal(item),
    0,
  );

  async function handleSubmit(
    event: React.FormEvent,
  ) {
    event.preventDefault();

    if (supplierId <= 0) {
      setError("Please select a supplier.");
      return;
    }

    if (!purchaseDate) {
      setError("Purchase date is required.");
      return;
    }

    if (items.length === 0) {
      setError("Add at least one ingredient.");
      return;
    }

    const invalidItem = items.some(
      (item) =>
        item.ingredientId <= 0 ||
        item.quantity <= 0 ||
        item.unitCost < 0,
    );

    if (invalidItem) {
      setError(
        "Each ingredient must have a valid quantity and unit cost.",
      );
      return;
    }

    try {
      setSaving(true);
      setError(null);

      await createPurchase({
        supplierId,
        purchaseDate,
        invoiceNumber: invoiceNumber.trim() || undefined,
        items,
      });

      await loadPurchases();

      closeForm();
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Unable to create purchase.",
      );
    } finally {
      setSaving(false);
    }
  }

  async function handleReceive(id: number) {
    const confirmed = window.confirm(
      "Are you sure you want to receive this purchase?",
    );

    if (!confirmed) {
      return;
    }

    try {
      setError(null);

      await receivePurchase(id);
      await loadPurchases();
    } catch {
      setError("Unable to receive purchase.");
    }
  }

  async function handleCancel(id: number) {
    const confirmed = window.confirm(
      "Are you sure you want to cancel this purchase?",
    );

    if (!confirmed) {
      return;
    }

    try {
      setError(null);

      await cancelPurchase(id);
      await loadPurchases();
    } catch {
      setError("Unable to cancel purchase.");
    }
  }

  if (loading) {
    return <p>Loading purchases...</p>;
  }

  return (
    <div>
      <div>
        <h1>Purchases</h1>

        {!showForm && (
          <button onClick={openNewPurchase}>
            + New Purchase
          </button>
        )}
      </div>

      {error && <p>{error}</p>}

      {showForm && (
        <form onSubmit={handleSubmit}>
          <h2>New Purchase</h2>

          <div>
            <label>Supplier</label>

            <select
              value={supplierId}
              onChange={(event) =>
                setSupplierId(Number(event.target.value))
              }
            >
              <option value={0}>
                Select supplier
              </option>

              {suppliers.map((supplier) => (
                <option
                  key={supplier.id}
                  value={supplier.id}
                >
                  {supplier.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label>Purchase Date</label>

            <input
              type="date"
              value={purchaseDate}
              onChange={(event) =>
                setPurchaseDate(event.target.value)
              }
            />
          </div>

          <div>
            <label>Invoice Number</label>

            <input
              value={invoiceNumber}
              onChange={(event) =>
                setInvoiceNumber(event.target.value)
              }
              placeholder="Optional"
            />
          </div>

          <h3>Items</h3>

          <table>
            <thead>
              <tr>
                <th>Ingredient</th>
                <th>Quantity</th>
                <th>Unit Cost</th>
                <th>Total</th>
                <th></th>
              </tr>
            </thead>

            <tbody>
              {items.map((item, index) => (
                <tr key={index}>
                  <td>
                    <select
                      value={item.ingredientId}
                      onChange={(event) =>
                        updateItem(
                          index,
                          "ingredientId",
                          Number(event.target.value),
                        )
                      }
                    >
                      <option value={0}>
                        Select ingredient
                      </option>

                      {ingredients.map((ingredient) => (
                        <option
                          key={ingredient.id}
                          value={ingredient.id}
                        >
                          {ingredient.name} ({ingredient.unit})
                        </option>
                      ))}
                    </select>
                  </td>

                  <td>
                    <input
                      type="number"
                      min="0"
                      step="0.001"
                      value={item.quantity}
                      onChange={(event) =>
                        updateItem(
                          index,
                          "quantity",
                          Number(event.target.value),
                        )
                      }
                    />
                  </td>

                  <td>
                    <input
                      type="number"
                      min="0"
                      step="0.01"
                      value={item.unitCost}
                      onChange={(event) =>
                        updateItem(
                          index,
                          "unitCost",
                          Number(event.target.value),
                        )
                      }
                    />
                  </td>

                  <td>
                    ₹{getItemTotal(item).toFixed(2)}
                  </td>

                  <td>
                    {items.length > 1 && (
                      <button
                        type="button"
                        onClick={() =>
                          removeItem(index)
                        }
                      >
                        Remove
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <br />

          <button
            type="button"
            onClick={addItem}
          >
            + Add Ingredient
          </button>

          <h3>
            Total: ₹{totalAmount.toFixed(2)}
          </h3>

          <button
            type="submit"
            disabled={saving}
          >
            {saving ? "Saving..." : "Save Purchase"}
          </button>

          <button
            type="button"
            onClick={closeForm}
            disabled={saving}
          >
            Cancel
          </button>
        </form>
      )}

      {!showForm && (
        <>
          {purchases.length === 0 ? (
            <p>No purchases found.</p>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Date</th>
                  <th>Supplier</th>
                  <th>Invoice</th>
                  <th>Total</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>

              <tbody>
                {purchases.map((purchase) => (
                  <tr key={purchase.id}>
                    <td>{purchase.purchaseDate}</td>

                    <td>{purchase.supplierName}</td>

                    <td>
                      {purchase.invoiceNumber || "-"}
                    </td>

                    <td>
                      ₹{purchase.totalAmount.toFixed(2)}
                    </td>

                    <td>{purchase.status}</td>

                    <td>
                      {purchase.status === "PENDING" && (
                        <>
                          <button
                            onClick={() =>
                              handleReceive(
                                purchase.id,
                              )
                            }
                          >
                            Receive
                          </button>

                          <button
                            onClick={() =>
                              handleCancel(
                                purchase.id,
                              )
                            }
                          >
                            Cancel
                          </button>
                        </>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </>
      )}
    </div>
  );
}

export default PurchasesPage;