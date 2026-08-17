import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { deleteRecipe, getRecipes } from "../api/recipesApi";
import type { Recipe } from "../types/recipe";

const currency = new Intl.NumberFormat("en-IN", {
  style: "currency",
  currency: "INR",
  maximumFractionDigits: 2,
});

function RecipesPage() {
  const [recipes, setRecipes] = useState<Recipe[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadRecipes() {
      try {
        setRecipes(await getRecipes());
      } catch {
        setError("Unable to load recipes.");
      } finally {
        setLoading(false);
      }
    }

    loadRecipes();
  }, []);

  if (loading) return <p>Loading recipes...</p>;

  async function handleDelete(id: number, productName: string) {
    if (!window.confirm(`Are you sure you want to delete the recipe for "${productName}"?`)) return;
    try {
      await deleteRecipe(id);
      setRecipes((current) => current.filter((recipe) => recipe.id !== id));
    } catch {
      setError("Unable to delete recipe.");
    }
  }

  return (
    <div>
      <h1>Recipes</h1>
      <Link to="/recipes/new">
        <button>+ Add Recipe</button>
      </Link>

      {error && <p>{error}</p>}

      {!error && recipes.length === 0 && <p>No recipes found.</p>}

      {!error && recipes.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Product</th>
              <th>Batch Yield</th>
              <th>Ingredients</th>
              <th>Batch Cost</th>
              <th>Cost per Item</th>
              <th>Gross Profit</th>
              <th>Margin %</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {recipes.map((recipe) => (
              <tr key={recipe.id}>
                <td>{recipe.productName}</td>
                <td>{recipe.yieldQuantity}</td>
                <td>{recipe.ingredients.length}</td>
                <td>{currency.format(recipe.totalCost)}</td>
                <td>{currency.format(recipe.costPerItem)}</td>
                <td>{currency.format(recipe.grossProfit)}</td>
                <td>{recipe.marginPercentage.toFixed(2)}%</td>
                <td>
                  <Link to={`/recipes/${recipe.id}/edit`}>Edit</Link>{" "}
                  <button onClick={() => handleDelete(recipe.id, recipe.productName)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default RecipesPage;
