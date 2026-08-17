import { useEffect, useMemo, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getIngredients } from "../api/ingredientsApi";
import { getProducts } from "../api/productsApi";
import { createRecipe, getRecipe, updateRecipe } from "../api/recipesApi";
import type { Ingredient } from "../types/ingredient";
import type { Product } from "../types/product";
import { calculateRecipeLineCost, compatibleUnits } from "../utils/units";

type RecipeLine = { ingredientId: number; quantity: number; unit: string };

const currency = new Intl.NumberFormat("en-IN", {
  style: "currency",
  currency: "INR",
  maximumFractionDigits: 2,
});

function AddRecipePage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditing = Boolean(id);
  const [products, setProducts] = useState<Product[]>([]);
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [productId, setProductId] = useState(0);
  const [yieldQuantity, setYieldQuantity] = useState(1);
  const [lines, setLines] = useState<RecipeLine[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadFormData() {
      try {
        const [productsData, ingredientsData, recipe] = await Promise.all([
          getProducts(),
          getIngredients(),
          id ? getRecipe(Number(id)) : Promise.resolve(null),
        ]);
        setProducts(productsData);
        setIngredients(ingredientsData);
        if (recipe) {
          setProductId(recipe.productId);
          setYieldQuantity(recipe.yieldQuantity);
          setLines(recipe.ingredients.map((line) => ({
            ingredientId: line.ingredientId,
            quantity: line.quantity,
            unit: line.unit,
          })));
        }
      } catch {
        setError("Unable to load the recipe form.");
      } finally {
        setLoading(false);
      }
    }

    loadFormData();
  }, [id]);

  const totalCost = useMemo(
    () => lines.reduce((total, line) => {
      const ingredient = ingredients.find((item) => item.id === line.ingredientId);
      return total + calculateRecipeLineCost(line.quantity, line.unit, ingredient?.unit ?? "", ingredient?.costPerUnit ?? 0);
    }, 0),
    [ingredients, lines],
  );

  const costPerItem = yieldQuantity > 0 ? totalCost / yieldQuantity : 0;

  const currentProduct = products.find((p) => p.id === productId);
  const sellingPrice = currentProduct?.sellingPrice ?? 0;
  const grossProfit = sellingPrice - costPerItem;
  const marginPercentage = sellingPrice > 0 ? (grossProfit / sellingPrice) * 100 : 0;

  function addIngredient() {
    const unusedIngredient = ingredients.find(
      (ingredient) => !lines.some((line) => line.ingredientId === ingredient.id),
    );

    if (unusedIngredient) {
      setLines((current) => [
        ...current,
        { ingredientId: unusedIngredient.id, quantity: 1, unit: unusedIngredient.unit },
      ]);
    }
  }

  function updateLine(index: number, changes: Partial<RecipeLine>) {
    setLines((current) => current.map((line, lineIndex) =>
      lineIndex === index ? { ...line, ...changes } : line,
    ));
  }

  function removeLine(index: number) {
    setLines((current) => current.filter((_, lineIndex) => lineIndex !== index));
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (productId <= 0 || yieldQuantity <= 0 || lines.length === 0) {
      setError("Select a product, enter a batch yield, and add at least one ingredient.");
      return;
    }

    setSaving(true);
    setError(null);

    try {
      if (id) await updateRecipe(Number(id), { productId, yieldQuantity, ingredients: lines });
      else await createRecipe({ productId, yieldQuantity, ingredients: lines });
      navigate("/recipes");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to save recipe.");
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <p>Loading recipe form...</p>;

  return (
    <div>
      <h1>{isEditing ? "Edit Recipe" : "Add Recipe"}</h1>
      {error && <p>{error}</p>}

      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="product">Product</label><br />
          <select id="product" value={productId} onChange={(event) => setProductId(Number(event.target.value))} required>
            <option value={0}>Select product</option>
            {products.map((product) => <option key={product.id} value={product.id}>{product.name}</option>)}
          </select>
        </div>

        <br />

        <div>
          <label htmlFor="yieldQuantity">Batch Yield</label><br />
          <input id="yieldQuantity" type="number" min="0.001" step="0.001" value={yieldQuantity} onChange={(event) => setYieldQuantity(Number(event.target.value))} required />
        </div>

        <br />

        <h2>Ingredients</h2>
        {ingredients.length === 0 && (
          <p>
            No active ingredients are available. <Link to="/ingredients/new">Add an ingredient first</Link>.
          </p>
        )}
        {lines.map((line, index) => {
          const ingredient = ingredients.find((item) => item.id === line.ingredientId);
          const lineCost = calculateRecipeLineCost(line.quantity, line.unit, ingredient?.unit ?? "", ingredient?.costPerUnit ?? 0);

          return (
            <div key={`${line.ingredientId}-${index}`}>
              <select value={line.ingredientId} onChange={(event) => {
                const nextIngredient = ingredients.find((item) => item.id === Number(event.target.value));
                if (nextIngredient) updateLine(index, { ingredientId: nextIngredient.id, unit: nextIngredient.unit });
              }}>
                {ingredients.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
              </select>{" "}
              <input aria-label="Quantity" type="number" min="0.001" step="0.001" value={line.quantity} onChange={(event) => updateLine(index, { quantity: Number(event.target.value) })} />
              <select aria-label="Recipe unit" value={line.unit} onChange={(event) => updateLine(index, { unit: event.target.value })}>
                {compatibleUnits(ingredient?.unit ?? "").map((unit) => <option key={unit} value={unit}>{unit}</option>)}
              </select>
              <span> — {currency.format(lineCost)}</span>{" "}
              <button type="button" onClick={() => removeLine(index)}>Remove</button>
            </div>
          );
        })}

        <br />
        <button type="button" onClick={addIngredient} disabled={ingredients.length === 0 || lines.length === ingredients.length}>+ Add Ingredient</button>
        <h3>Cost Summary</h3>
        <p>Batch cost: {currency.format(totalCost)}</p>
        <p>Cost per item: {currency.format(costPerItem)}</p>
        {currentProduct && (
          <>
            <p>Selling price: {currency.format(sellingPrice)}</p>
            <p>Gross profit: {currency.format(grossProfit)}</p>
            <p>Margin: {marginPercentage.toFixed(2)}%</p>
          </>
        )}

        <button type="submit" disabled={saving}>{saving ? "Saving..." : isEditing ? "Update Recipe" : "Save Recipe"}</button>{" "}
        <button type="button" onClick={() => navigate("/recipes")}>Cancel</button>
      </form>
    </div>
  );
}

export default AddRecipePage;
