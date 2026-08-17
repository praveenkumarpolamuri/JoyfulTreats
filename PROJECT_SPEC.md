# JoyfulTreats - Project Specification

## 1. Project Overview

JoyfulTreats is a business management application for a home-based bakery/cookie business.

The application should help manage:

- Product catalog
- Product categories
- Ingredients
- Recipes
- Recipe costing
- Inventory
- Purchases
- Customers
- Sales orders
- Expenses
- Production batches
- Business dashboard
- Reports

The application is also a learning project for a senior .NET developer preparing for interviews.

The implementation must therefore prioritize:

- Clean architecture
- Maintainable code
- Strong domain modeling
- Proper EF Core usage
- RESTful API design
- React best practices
- TypeScript
- Testability
- Clear separation of concerns

---

# 2. Technology Stack

## Backend

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core 10
- PostgreSQL 17
- Npgsql
- Swagger / OpenAPI
- Docker Compose

## Frontend

- React
- TypeScript
- Vite
- React Router
- ESLint

## Database

PostgreSQL is the primary database.

Do NOT introduce SQL Server.

The application should remain portable enough that another relational database can be introduced later if required.

---

# 3. Existing Solution Structure

The existing solution is:

JoyfulTreats.slnx

Projects:

JoyfulTreats.Api
JoyfulTreats.Application
JoyfulTreats.Domain
JoyfulTreats.Infrastructure

Frontend:

JoyfulTreats.web

Existing project dependencies:

JoyfulTreats.Api
    -> JoyfulTreats.Application
    -> JoyfulTreats.Infrastructure

JoyfulTreats.Application
    -> JoyfulTreats.Domain

JoyfulTreats.Infrastructure
    -> JoyfulTreats.Application
    -> JoyfulTreats.Domain

Domain must not depend on Application, Infrastructure, or API.

Application must not depend on Infrastructure.

---

# 4. Architecture

Use Clean Architecture principles.

Dependency direction:

Domain
    ^
Application
    ^
Infrastructure

API depends on Application and Infrastructure for composition/root setup.

Conceptually:

React
    |
    | HTTP/JSON
    v
ASP.NET Core API
    |
    v
Application Services
    |
    v
Application Interfaces
    |
    v
Infrastructure
    |
    v
EF Core
    |
    v
PostgreSQL

---

# 5. Backend Layer Responsibilities

## Domain

Contains:

- Entities
- Domain concepts
- Domain-level rules

Do not put:

- EF Core code
- DbContext
- API logic
- HTTP logic
- React-specific logic

in the Domain project.

---

## Application

Contains:

- DTOs
- Application interfaces
- Services
- Business use cases
- Persistence abstractions

Application services should coordinate business operations.

---

## Infrastructure

Contains:

- EF Core DbContext
- Entity configurations
- Database access
- EF Core migrations
- External infrastructure implementations

---

## API

Contains:

- Controllers
- HTTP-specific behavior
- Dependency injection setup
- Middleware
- Authentication configuration
- Swagger configuration

Controllers should remain thin.

Business logic belongs in Application services.

---

# 6. Database Entities

## Category

Fields:

- Id
- Name
- Description
- IsActive
- CreatedAt

Relationship:

Category 1 -> many Products

---

## Product

Fields:

- Id
- Name
- SKU
- CategoryId
- SellingPrice
- MRP
- IsActive
- CreatedAt
- UpdatedAt

Relationships:

Product -> Category

Product -> Recipe

Products use soft delete.

Do not physically delete business products.

Use:

IsActive = false

---

## Ingredient

Fields:

- Id
- Name
- Unit
- CostPerUnit
- IsActive
- CreatedAt
- UpdatedAt

Examples:

Almonds
Unit = g
CostPerUnit = 0.88

Oats
Unit = g
CostPerUnit = 0.175

Milk
Unit = ml
CostPerUnit = 0.10

Use decimal for monetary values.

Do not use double for money.

---

## Recipe

Fields:

- Id
- ProductId
- BatchSize
- YieldQuantity
- CreatedAt
- UpdatedAt

Relationship:

Product 1 -> 1 Recipe

Recipe 1 -> many RecipeIngredients

---

## RecipeIngredient

Fields:

- Id
- RecipeId
- IngredientId
- Quantity

Relationships:

RecipeIngredient -> Recipe

RecipeIngredient -> Ingredient

This represents a many-to-many relationship between Recipe and Ingredient with Quantity as relationship data.

---

## InventoryStock

Fields:

- Id
- IngredientId
- Quantity
- Unit
- ReorderLevel
- UpdatedAt

---

## InventoryTransaction

Fields:

- Id
- IngredientId
- TransactionType
- Quantity
- ReferenceId
- CreatedAt

Transaction types:

- PURCHASE
- PRODUCTION
- ADJUSTMENT
- WASTAGE

---

## Supplier

Fields:

- Id
- Name
- Phone
- Email
- Address
- IsActive
- CreatedAt

---

## Purchase

Fields:

- Id
- SupplierId
- PurchaseDate
- InvoiceNumber
- TotalAmount
- Status
- CreatedAt

---

## PurchaseItem

Fields:

- Id
- PurchaseId
- IngredientId
- Quantity
- UnitCost
- TotalCost

---

## Customer

Fields:

- Id
- Name
- Phone
- Email
- Address
- IsActive
- CreatedAt

---

## Order

Fields:

- Id
- CustomerId
- OrderDate
- Status
- Subtotal
- Discount
- Tax
- TotalAmount
- PaymentStatus
- CreatedAt

Order statuses:

- PENDING
- CONFIRMED
- IN_PRODUCTION
- READY
- DELIVERED
- CANCELLED

---

## OrderItem

Fields:

- Id
- OrderId
- ProductId
- Quantity
- UnitPrice
- TotalPrice

---

## Expense

Fields:

- Id
- Category
- Description
- Amount
- ExpenseDate
- Notes

Use decimal for Amount.

---

## ProductionBatch

Fields:

- Id
- ProductId
- RecipeId
- PlannedQuantity
- ProducedQuantity
- ProductionDate
- Status
- Notes

Production statuses:

- PLANNED
- IN_PROGRESS
- COMPLETED
- CANCELLED

---

# 7. Recipe Costing

Recipe costing is a core business feature.

For each RecipeIngredient:

IngredientCost:

Quantity * Ingredient.CostPerUnit

RecipeCost:

SUM(IngredientCost)

CostPerUnit:

RecipeCost / Recipe.YieldQuantity

Example:

Almond Chocolate Cookie:

Almonds = 75g
Oats = 150g
Atta = 150g
Jaggery = 150g
Butter = 100g
Cocoa = 30g
Dark Chocolate = 60g
Milk = 50ml

If total recipe cost is:

₹268.68

and yield is:

27 cookies

then:

Cost per cookie = ₹268.68 / 27

Approximately:

₹9.95

The application should calculate:

- Recipe Cost
- Cost Per Unit
- Selling Price
- Gross Profit
- Gross Margin %

---

# 8. API Design

## Categories

GET    /api/Categories
GET    /api/Categories/{id}
POST   /api/Categories
PUT    /api/Categories/{id}
DELETE /api/Categories/{id}

---

## Products

GET    /api/Products
GET    /api/Products/{id}
POST   /api/Products
PUT    /api/Products/{id}
DELETE /api/Products/{id}

DELETE should perform soft delete.

---

## Ingredients

GET    /api/Ingredients
GET    /api/Ingredients/{id}
POST   /api/Ingredients
PUT    /api/Ingredients/{id}
DELETE /api/Ingredients/{id}

DELETE should perform soft delete.

---

## Recipes

GET    /api/Recipes
GET    /api/Recipes/{id}
POST   /api/Recipes
PUT    /api/Recipes/{id}
DELETE /api/Recipes/{id}

---

## Inventory

GET    /api/Inventory
GET    /api/Inventory/{ingredientId}
POST   /api/Inventory/adjust

---

## Purchases

GET    /api/Purchases
GET    /api/Purchases/{id}
POST   /api/Purchases

---

## Customers

GET    /api/Customers
GET    /api/Customers/{id}
POST   /api/Customers
PUT    /api/Customers/{id}
DELETE /api/Customers/{id}

---

## Orders

GET    /api/Orders
GET    /api/Orders/{id}
POST   /api/Orders
PUT    /api/Orders/{id}

---

## Dashboard

GET /api/Dashboard/summary
GET /api/Dashboard/sales
GET /api/Dashboard/products
GET /api/Dashboard/expenses

---

# 9. DTO Rules

Do not expose EF Core entities directly from API endpoints.

Use DTOs.

For example:

ProductDto
CreateProductDto
UpdateProductDto

IngredientDto
CreateIngredientDto
UpdateIngredientDto

RecipeDto
CreateRecipeDto
UpdateRecipeDto

DTOs should contain only data required by the API contract.

---

# 10. EF Core Rules

Use:

- Async methods
- CancellationToken
- AsNoTracking for read-only queries
- LINQ projections where appropriate
- Explicit relationships
- Fluent API for important relationship/configuration rules
- Decimal precision for money
- Database indexes for frequently queried fields

Avoid unnecessary:

- Include()
- Tracking for read-only queries
- N+1 queries
- In-memory filtering of large datasets

Prefer:

_context.Products
    .AsNoTracking()
    .Where(...)
    .Select(...)
    .ToListAsync(cancellationToken)

over loading entire entities unnecessarily.

---

# 11. Soft Delete

Products, Ingredients, Categories, Customers and other appropriate master data should use:

IsActive

Normal GET endpoints should return active records unless explicitly requested otherwise.

Never physically delete historical business records unless there is a strong reason.

---

# 12. React Architecture

Frontend structure:

src/
    api/
    components/
    pages/
    types/
    hooks/
    layouts/
    routes/
    utils/

API modules:

productsApi.ts
categoriesApi.ts
ingredientsApi.ts
recipesApi.ts
inventoryApi.ts
ordersApi.ts
customersApi.ts

---

# 13. React Rules

Use functional components.

Use TypeScript.

Use:

useState

for local component state.

Use:

useEffect

for synchronization with external systems such as initial API loading.

Use React Router for navigation.

Do not use window.location for normal internal navigation.

Use Link or navigate().

---

# 14. API Layer

React pages should not contain raw fetch calls everywhere.

Prefer:

productsApi.ts

with:

getProducts()
getProduct(id)
createProduct()
updateProduct()
deleteProduct()

Pages call API functions.

---

# 15. Loading and Error States

Every API-driven page should consider:

- Loading
- Error
- Empty
- Success

Example:

Loading products...

Unable to load products.

No products found.

Products displayed successfully.

---

# 16. Forms

Use controlled React forms initially.

Every form should:

- Validate required fields
- Disable submit while saving
- Show useful errors
- Avoid duplicate submission
- Navigate after successful save

---

# 17. UI Modules

Main navigation:

Dashboard
Products
Ingredients
Recipes
Inventory
Purchases
Customers
Orders
Expenses
Reports

---

# 18. Dashboard

Dashboard should eventually show:

Today's Sales

Monthly Sales

Number of Active Products

Low Stock Ingredients

Pending Orders

Monthly Expenses

Estimated Profit

Charts:

- Sales by day
- Sales by product
- Top products
- Expenses
- Profit

---

# 19. Development Phases

## Phase 1 - Foundation

COMPLETED

- Solution
- Clean Architecture
- PostgreSQL
- Docker
- EF Core
- Migrations
- Swagger
- React
- TypeScript
- Vite
- CORS

---

## Phase 2 - Products

COMPLETED

- Category API
- Product API
- Product list
- Add Product
- Edit Product
- Soft Delete
- React API integration

---

# Phase 3 - Ingredients

CURRENT PHASE

Implement:

Backend:

- Ingredient entity
- DTOs
- EF Core configuration
- Migration
- IApplicationDbContext
- Ingredient service
- Ingredient interface
- Ingredient controller
- CRUD
- Soft delete

Frontend:

- Ingredient type
- ingredientsApi.ts
- IngredientsPage
- Add Ingredient
- Edit Ingredient
- Delete Ingredient
- Loading state
- Error state
- Empty state

Acceptance criteria:

1. User can view active ingredients.
2. User can create ingredient.
3. User can edit ingredient.
4. User can deactivate ingredient.
5. Deactivated ingredients remain in database.
6. CostPerUnit uses decimal.
7. API does not expose EF entities directly.
8. React does not directly call fetch from multiple components.
9. Application builds successfully.
10. Migration succeeds.

---

# Phase 4 - Recipes

Implement:

- Recipe entity
- RecipeIngredient
- Relationships
- DTOs
- Recipe service
- CRUD
- Recipe editor
- Ingredient selection
- Quantity entry

UI:

Product:
Almond Chocolate Cookie

Ingredients:

Almonds       75g
Oats          150g
Atta          150g
Jaggery       150g
Butter        100g

[Add Ingredient]

Show:

Recipe Cost
Cost Per Cookie
Yield

---

# Phase 5 - Recipe Costing

Implement:

- Ingredient cost calculation
- Recipe cost
- Unit cost
- Gross profit
- Margin %

All monetary calculations must use decimal.

---

# Phase 6 - Inventory

Implement:

- Current stock
- Purchases
- Stock adjustments
- Wastage
- Production consumption
- Low-stock alerts
- Inventory transactions

---

# Phase 7 - Customers

Implement:

- Customer CRUD
- Search
- Customer order history

---

# Phase 8 - Orders

Implement:

- Create order
- Add products
- Calculate totals
- Discounts
- Tax
- Payment status
- Order status
- Customer association

---

# Phase 9 - Production

Implement:

- Production batch
- Recipe selection
- Planned quantity
- Ingredient consumption
- Produced quantity
- Production status

---

# Phase 10 - Expenses

Implement:

- Expense CRUD
- Expense categories
- Monthly expense summary

---

# Phase 11 - Dashboard

Implement:

- Sales summary
- Expense summary
- Profit summary
- Low stock
- Pending orders
- Top products
- Charts

---

# Phase 12 - Authentication

Implement:

- Authentication
- JWT
- Users
- Roles

Roles:

Admin
Manager
Staff

Authorization should be applied at API level.

---

# Phase 13 - Testing

Backend:

- Unit tests
- Integration tests
- Service tests
- API tests

Frontend:

- Component tests
- API mocking
- Form validation tests

---

# 20. Definition of Done

A feature is not complete until:

1. Domain entity exists.
2. Database relationship/configuration exists.
3. Migration is created successfully.
4. Application service exists.
5. Interface exists.
6. DTOs exist.
7. Controller endpoint exists.
8. Swagger endpoint works.
9. React API module exists.
10. React UI exists.
11. Loading state exists.
12. Error state exists.
13. Empty state exists where appropriate.
14. Build succeeds.
15. Existing functionality still works.

---

# 21. Important Development Rule

Do not implement the entire application in one operation.

Implement one phase at a time.

Before starting a phase:

1. Read this specification.
2. Inspect the existing implementation.
3. Follow existing conventions.
4. Avoid unnecessary refactoring.
5. Implement only the requested phase.
6. Build the solution.
7. Fix compilation errors.
8. Verify API endpoints.
9. Verify frontend.
10. Report files changed.

Never replace working architecture unnecessarily.

---

# 22. Current Status

Completed:

- Foundation
- PostgreSQL
- EF Core
- Categories
- Products CRUD
- Product soft delete
- React product management

Current phase:

Phase 3 - Ingredients

Next:

Phase 4 - Recipes

Then:

Phase 5 - Recipe Costing
Phase 6 - Inventory
Phase 7 - Customers
Phase 8 - Orders
Phase 9 - Production
Phase 10 - Expenses
Phase 11 - Dashboard
Phase 12 - Authentication
Phase 13 - Testing