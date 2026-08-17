# JoyfulTreats Coding Guidelines

## 1. General Principles

Write production-quality code.

Prefer:

- Simple
- Readable
- Maintainable
- Testable
- Explicit

Avoid unnecessary abstractions.

Do not introduce libraries unless there is a clear reason.

Do not rewrite existing working code without a reason.

---

# 2. C# Naming

Use PascalCase for:

- Classes
- Interfaces
- Methods
- Properties
- Enums

Examples:

ProductService
IProductService
GetProductsAsync
SellingPrice

Use camelCase for:

- Local variables
- Parameters

Examples:

product
productId
cancellationToken

Interfaces must start with:

I

Example:

IProductService

---

# 3. C# Async

Use async APIs for database operations.

Preferred:

await _context.Products
    .ToListAsync(cancellationToken);

Avoid synchronous EF operations inside API requests.

Always propagate CancellationToken where appropriate.

---

# 4. EF Core

Use:

AsNoTracking()

for read-only queries.

Prefer projection:

.Select(...)

when only DTO data is required.

Avoid loading unnecessary columns/entities.

Do not expose EF Core entities directly from controllers.

---

# 5. Money

Always use:

decimal

for:

- Price
- MRP
- Cost
- Revenue
- Expense
- Profit
- Tax
- Discount

Never use:

double

for financial calculations.

Configure database precision explicitly.

Example:

HasPrecision(18, 2)

---

# 6. Database Relationships

Use explicit foreign keys.

Example:

public int CategoryId { get; set; }

public Category Category { get; set; } = null!;

Use Fluent API for relationship configuration where appropriate.

---

# 7. DTOs

Never return database entities directly from API endpoints.

Use DTOs.

Example:

ProductDto
CreateProductDto
UpdateProductDto

---

# 8. Controllers

Controllers should be thin.

Good:

Controller
    ↓
Application Service
    ↓
DbContext

Avoid putting business logic directly inside controllers.

Controllers are responsible for:

- HTTP input
- Calling application services
- HTTP response
- Status codes

---

# 9. Service Layer

Business logic belongs in Application services.

Example:

ProductService
IngredientService
RecipeService

Services should validate business rules.

---

# 10. Error Handling

Do not silently swallow exceptions.

Avoid:

catch
{
}

Prefer meaningful error handling.

Do not expose database exception details to clients.

---

# 11. HTTP Status Codes

Use appropriate status codes.

200 OK
Successful GET/update where applicable.

201 Created
Successful creation.

204 No Content
Successful delete/deactivation when appropriate.

400 Bad Request
Invalid input.

404 Not Found
Resource does not exist.

409 Conflict
Business conflict.

500 Internal Server Error
Unexpected server failure.

---

# 12. React

Use functional components.

Use TypeScript.

Avoid `any`.

Prefer explicit types.

Bad:

const data: any = ...

Good:

const data: Product[] = ...

---

# 13. React State

Use useState for local state.

Example:

const [products, setProducts] = useState<Product[]>([]);

Use functional updates when the new state depends on the previous state.

Example:

setProducts(current =>
    current.filter(product => product.id !== id)
);

---

# 14. useEffect

Use useEffect for synchronization with external systems.

Typical example:

useEffect(() => {
    loadProducts();
}, []);

Do not use useEffect unnecessarily for simple calculations.

---

# 15. API Calls

Do not put raw fetch calls throughout components.

Create API modules.

Example:

productsApi.ts

Then:

getProducts()
getProduct(id)
createProduct()
updateProduct()
deleteProduct()

Components/pages should call those functions.

---

# 16. React Navigation

Use React Router.

Prefer:

<Link to="/products">

or:

navigate("/products")

Do not use:

window.location.href

for normal internal navigation.

---

# 17. Forms

Use controlled inputs.

Example:

<input
    name="name"
    value={form.name}
    onChange={handleChange}
/>

Disable submit while saving.

Example:

<button disabled={saving}>

Avoid duplicate API submissions.

---

# 18. Loading/Error/Empty States

Every API-driven page should handle:

Loading

Error

Empty

Success

Example:

if (loading)
    return <p>Loading...</p>;

if (error)
    return <p>{error}</p>;

if (items.length === 0)
    return <p>No records found.</p>;

---

# 19. React Components

Keep components reasonably small.

If a component becomes too large, extract reusable components.

Examples:

ProductTable
ProductForm
IngredientTable
IngredientForm
RecipeIngredientEditor

Do not prematurely create dozens of abstractions.

---

# 20. API URLs

Keep API configuration centralized.

Do not hardcode different API URLs throughout components.

Prefer:

const API_URL = ...

in API/configuration layer.

Later move this to environment configuration.

---

# 21. File Organization

Backend:

Domain/Entities

Application/DTOs

Application/Services

Application/Interfaces

Infrastructure/Persistence

Infrastructure/Configurations

Api/Controllers

Frontend:

src/api

src/components

src/pages

src/types

src/hooks

src/utils

---

# 22. Database Migrations

After entity/schema changes:

1. Build solution.
2. Create migration.
3. Review migration.
4. Apply migration.
5. Verify database.

Do not manually modify migration files unless necessary.

Migration naming should be meaningful.

Examples:

InitialCreate
AddIngredients
AddRecipes
AddInventory
AddOrders

---

# 23. Soft Delete

For master/business entities where historical references matter:

Use:

IsActive = false

Do not physically delete records.

Normal queries should filter inactive records unless an administrative/inactive view is specifically requested.

---

# 24. Query Performance

Avoid:

Loading all records and filtering in memory.

Bad:

var products = await _context.Products.ToListAsync();

var active = products.Where(...);

Prefer:

var products = await _context.Products
    .Where(...)
    .ToListAsync(cancellationToken);

Let PostgreSQL perform filtering.

---

# 25. N+1 Queries

Avoid repeatedly querying related entities inside loops.

Bad:

foreach (var product in products)
{
    await GetCategory(product.CategoryId);
}

Prefer:

- Projection
- Include where appropriate
- Proper joins

---

# 26. Security

Never commit:

- Passwords
- API keys
- JWT secrets
- Database passwords
- Connection secrets

Use configuration/environment variables.

---

# 27. Git

Use small meaningful commits.

Examples:

feat: add ingredient entity

feat: add ingredient CRUD API

feat: add ingredient management UI

feat: add recipe costing

fix: correct product soft delete

---

# 28. Codex Rules

Before making changes:

1. Read PROJECT_SPEC.md.
2. Read CODING_GUIDELINES.md.
3. Inspect existing code.
4. Understand current architecture.
5. Reuse existing patterns.

Do not assume a file or method exists.

Search before creating duplicates.

Do not replace working implementations without justification.

Implement only the requested feature.

---

# 29. Codex Workflow

For every feature:

STEP 1
Inspect existing implementation.

STEP 2
Plan changes.

STEP 3
Implement backend.

STEP 4
Implement migration.

STEP 5
Implement API.

STEP 6
Implement React API module.

STEP 7
Implement React UI.

STEP 8
Build backend.

STEP 9
Build frontend.

STEP 10
Report:

- Files created
- Files modified
- Migration created
- API endpoints
- Tests/build results
- Any remaining issues

---

# 30. Do Not Over-Engineer

Do not introduce:

- Repository pattern
- CQRS
- MediatR
- Redux
- Generic repositories
- Event sourcing
- Microservices

unless the project specifically requires them.

The goal is a clean, understandable modular monolith.

---

# 31. Learning Requirement

Whenever implementing an important concept, explain it.

Important concepts include:

- EF Core relationships
- LINQ
- AsNoTracking
- DTO projection
- Dependency Injection
- React useState
- React useEffect
- Controlled forms
- React Router
- API communication
- Transactions
- Async/await

The developer should be able to explain the implementation during a senior .NET interview.

---

# 32. Current Task

Current implementation phase:

PHASE 3 - INGREDIENTS

Implement only Ingredient CRUD.

Do not implement Recipes, Inventory, Orders, Dashboard or Authentication yet.

After implementation:

- Build backend
- Build frontend
- Verify Swagger
- Verify React
- Verify soft delete
- Report results

Wait for the next instruction before implementing the next phase.