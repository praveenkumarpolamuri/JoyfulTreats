import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { createIngredient, getIngredient, updateIngredient, type CreateIngredientRequest } from "../api/ingredientsApi";
import { unitOptions } from "../utils/units";

function AddIngredientPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditing = Boolean(id);
  const [form, setForm] = useState<CreateIngredientRequest>({
    name: "",
    unit: "g",
    costPerUnit: 0,
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadIngredient() {
      if (!id) return;
      try {
        const ingredient = await getIngredient(Number(id));
        setForm({ name: ingredient.name, unit: ingredient.unit, costPerUnit: ingredient.costPerUnit });
      } catch {
        setError("Unable to load ingredient.");
      }
    }
    loadIngredient();
  }, [id]);

  function handleChange(event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) {
    const { name, value } = event.target;
    setForm((current) => ({
      ...current,
      [name]: name === "costPerUnit" ? Number(value) : value,
    }));
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError(null);

    try {
      if (id) await updateIngredient(Number(id), form);
      else await createIngredient(form);
      navigate("/ingredients");
    } catch {
      setError("Unable to create ingredient.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div>
      <h1>{isEditing ? "Edit Ingredient" : "Add Ingredient"}</h1>
      {error && <p>{error}</p>}
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="name">Ingredient Name</label><br />
          <input id="name" name="name" value={form.name} onChange={handleChange} required />
        </div>
        <br />
        <div>
          <label htmlFor="unit">Purchase Unit</label><br />
          <select id="unit" name="unit" value={form.unit} onChange={handleChange} required>
            {unitOptions.map((unit) => <option key={unit} value={unit}>{unit}</option>)}
          </select>
        </div>
        <br />
        <div>
          <label htmlFor="costPerUnit">Cost per Purchase Unit</label><br />
          <input id="costPerUnit" name="costPerUnit" type="number" min="0" step="0.0001" value={form.costPerUnit} onChange={handleChange} required />
        </div>
        <br />
        <button type="submit" disabled={saving}>{saving ? "Saving..." : isEditing ? "Update Ingredient" : "Save Ingredient"}</button>{" "}
        <button type="button" onClick={() => navigate("/ingredients")}>Cancel</button>
      </form>
    </div>
  );
}

export default AddIngredientPage;
