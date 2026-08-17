import { useEffect, useState } from "react";
import {
  createSupplier,
  deleteSupplier,
  getSuppliers,
  updateSupplier,
  type CreateSupplierRequest,
  type Supplier,
} from "../api/suppliersApi";

function SuppliersPage() {
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);

  const [form, setForm] = useState<CreateSupplierRequest>({
    name: "",
    phone: "",
    email: "",
    address: "",
  });

  async function loadSuppliers() {
    try {
      setLoading(true);
      setError(null);

      const data = await getSuppliers();
      setSuppliers(data);
    } catch {
      setError("Unable to load suppliers.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadSuppliers();
  }, []);

  function resetForm() {
    setForm({
      name: "",
      phone: "",
      email: "",
      address: "",
    });

    setEditingId(null);
    setShowForm(false);
    setError(null);
  }

  function handleChange(
    event: React.ChangeEvent<HTMLInputElement>,
  ) {
    const { name, value } = event.target;

    setForm((current) => ({
      ...current,
      [name]: value,
    }));
  }

  function handleAdd() {
    resetForm();
    setShowForm(true);
  }

  function handleEdit(supplier: Supplier) {
    setForm({
      name: supplier.name,
      phone: supplier.phone ?? "",
      email: supplier.email ?? "",
      address: supplier.address ?? "",
    });

    setEditingId(supplier.id);
    setShowForm(true);
    setError(null);
  }

  async function handleSubmit(
    event: React.FormEvent,
  ) {
    event.preventDefault();

    if (!form.name.trim()) {
      setError("Supplier name is required.");
      return;
    }

    try {
      setError(null);

      if (editingId === null) {
        await createSupplier(form);
      } else {
        await updateSupplier(editingId, form);
      }

      resetForm();
      await loadSuppliers();
    } catch {
      setError(
        editingId === null
          ? "Unable to create supplier."
          : "Unable to update supplier.",
      );
    }
  }

  async function handleDelete(id: number) {
    const confirmed = window.confirm(
      "Are you sure you want to deactivate this supplier?",
    );

    if (!confirmed) {
      return;
    }

    try {
      setError(null);

      await deleteSupplier(id);
      await loadSuppliers();
    } catch {
      setError("Unable to deactivate supplier.");
    }
  }

  if (loading) {
    return <p>Loading suppliers...</p>;
  }

  return (
    <div>
      <div>
        <h1>Suppliers</h1>

        {!showForm && (
          <button onClick={handleAdd}>
            + Add Supplier
          </button>
        )}
      </div>

      {error && <p>{error}</p>}

      {showForm && (
        <form onSubmit={handleSubmit}>
          <h2>
            {editingId === null
              ? "Add Supplier"
              : "Edit Supplier"}
          </h2>

          <div>
            <label>Name</label>
            <input
              name="name"
              value={form.name}
              onChange={handleChange}
              required
            />
          </div>

          <div>
            <label>Phone</label>
            <input
              name="phone"
              value={form.phone}
              onChange={handleChange}
            />
          </div>

          <div>
            <label>Email</label>
            <input
              name="email"
              type="email"
              value={form.email}
              onChange={handleChange}
            />
          </div>

          <div>
            <label>Address</label>
            <input
              name="address"
              value={form.address}
              onChange={handleChange}
            />
          </div>

          <button type="submit">
            {editingId === null ? "Save" : "Update"}
          </button>

          <button
            type="button"
            onClick={resetForm}
          >
            Cancel
          </button>
        </form>
      )}

      {suppliers.length === 0 ? (
        <p>No suppliers found.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Phone</th>
              <th>Email</th>
              <th>Address</th>
              <th>Actions</th>
            </tr>
          </thead>

          <tbody>
            {suppliers.map((supplier) => (
              <tr key={supplier.id}>
                <td>{supplier.name}</td>
                <td>{supplier.phone || "-"}</td>
                <td>{supplier.email || "-"}</td>
                <td>{supplier.address || "-"}</td>

                <td>
                  <button
                    onClick={() => handleEdit(supplier)}
                  >
                    Edit
                  </button>

                  <button
                    onClick={() => handleDelete(supplier.id)}
                  >
                    Deactivate
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

export default SuppliersPage;