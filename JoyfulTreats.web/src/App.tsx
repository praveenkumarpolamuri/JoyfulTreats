import { Link, Route, Routes } from "react-router-dom";
import DashboardPage from "./pages/DashboardPage";
import ProductsPage from "./pages/ProductsPage";
import AddProductPage from "./pages/AddProductPage";
import EditProductPage from "./pages/EditProductPage";
import RecipesPage from "./pages/RecipesPage";
import AddRecipePage from "./pages/AddRecipePage";
import IngredientsPage from "./pages/IngredientsPage";
import AddIngredientPage from "./pages/AddIngredientPage";
import SalesPage from "./pages/SalesPage";
import SaveSalePage from "./pages/SaveSalePage";
import SuppliersPage from "./pages/SuppliersPage";
import PurchasesPage from "./pages/PurchasesPage";

function App() {
  return (
    <>
      <nav>
  <Link to="/">Dashboard</Link>{" "}
  <Link to="/products">Products</Link>{" "}
  <Link to="/recipes">Recipes</Link>{" "}
  <Link to="/ingredients">Ingredients</Link>{" "}
  <Link to="/sales">Sales</Link>{" "}
  <Link to="/suppliers">Suppliers</Link>{" "}
  <Link to="/purchases">Purchases</Link>
</nav>

      <Routes>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/products" element={<ProductsPage />} />
        <Route path="/products/new" element={<AddProductPage />} />
        <Route path="/products/:id/edit" element={<EditProductPage />} />
        <Route path="/recipes" element={<RecipesPage />} />
        <Route path="/recipes/new" element={<AddRecipePage />} />
        <Route path="/recipes/:id/edit" element={<AddRecipePage />} />
        <Route path="/ingredients" element={<IngredientsPage />} />
        <Route path="/ingredients/new" element={<AddIngredientPage />} />
        <Route path="/ingredients/:id/edit" element={<AddIngredientPage />} />
        <Route path="/sales" element={<SalesPage />} />
        <Route path="/sales/new" element={<SaveSalePage />} />
        <Route path="/sales/:id/edit" element={<SaveSalePage />} />
        <Route path="/suppliers" element={<SuppliersPage />} />
        <Route path="/purchases" element={<PurchasesPage />} />


      </Routes>
    </>
  );
}

export default App;
