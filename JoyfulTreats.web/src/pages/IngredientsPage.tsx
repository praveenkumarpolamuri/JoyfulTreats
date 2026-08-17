import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { deleteIngredient, getIngredients } from "../api/ingredientsApi";
import type { Ingredient } from "../types/ingredient";

const currency = new Intl.NumberFormat("en-IN", {
  style: "currency",
  currency: "INR",
  maximumFractionDigits: 2,
});

function IngredientsPage() {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadIngredients() {
      try {
        setIngredients(await getIngredients());
      } catch {
        setError("Unable to load ingredients.");
      } finally {
        setLoading(false);
      }
    }

    loadIngredients();
  }, []);

  if (loading) return <p>Loading ingredients...</p>;

  async function handleDelete(id: number, name: string) {
    if (!window.confirm(`Are you sure you want to deactivate "${name}"?`)) return;
    try {
      await deleteIngredient(id);
      setIngredients((current) => current.filter((ingredient) => ingredient.id !== id));
    } catch {
      setError("Unable to delete ingredient.");
    }
  }

  return (
    <div>
      <h1>Ingredients</h1>
      <Link to="/ingredients/new"><button>+ Add Ingredient</button></Link>
      {error && <p>{error}</p>}
      {!error && ingredients.length === 0 && <p>No ingredients found.</p>}
      {!error && ingredients.length > 0 && (
        <table>
          <thead>
            <tr><th>Ingredient</th><th>Purchase Unit</th><th>Cost per Purchase Unit</th><th>Actions</th></tr>
          </thead>
          <tbody>
            {ingredients.map((ingredient) => (
              <tr key={ingredient.id}>
                <td>{ingredient.name}</td>
                <td>{ingredient.unit}</td>
                <td>{currency.format(ingredient.costPerUnit)}</td>
                <td>
                  <Link to={`/ingredients/${ingredient.id}/edit`}>Edit</Link>{" "}
                  <button onClick={() => handleDelete(ingredient.id, ingredient.name)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default IngredientsPage;
