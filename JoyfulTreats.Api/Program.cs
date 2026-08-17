using JoyfulTreats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using JoyfulTreats.Application.Interfaces.Persistence;
using JoyfulTreats.Application.Services.Categories;
using JoyfulTreats.Application.Services.Products;
using JoyfulTreats.Application.Services.Ingredients;
using JoyfulTreats.Application.Services.Recipes;
using JoyfulTreats.Application.Services.Sales;
using JoyfulTreats.Application.Services.Suppliers;
using JoyfulTreats.Application.Services.Inventory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped<IApplicationDbContext>(
    provider => provider.GetRequiredService<AppDbContext>());

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("JoyfulTreatsWeb", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("JoyfulTreatsWeb");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
