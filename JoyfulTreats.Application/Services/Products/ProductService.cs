using JoyfulTreats.Application.DTOs.Products;
using JoyfulTreats.Application.Interfaces.Persistence;
using JoyfulTreats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoyfulTreats.Application.Services.Products;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;

    public ProductService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                SellingPrice = product.SellingPrice,
                MRP = product.MRP,
                IsActive = product.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                SellingPrice = product.SellingPrice,
                MRP = product.MRP,
                IsActive = product.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductDto request,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            throw new InvalidOperationException(
                "The specified category does not exist.");
        }

        var product = new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            CategoryId = request.CategoryId,
            SellingPrice = request.SellingPrice,
            MRP = request.MRP,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(product.Id, cancellationToken)
            ?? throw new InvalidOperationException(
                "Product could not be retrieved after creation.");
    }

    public async Task<ProductDto?> UpdateAsync(
        int id,
        UpdateProductDto request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        var categoryExists = await _context.Categories
            .AnyAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            throw new InvalidOperationException(
                "The specified category does not exist.");
        }

        product.Name = request.Name;
        product.SKU = request.Sku;
        product.CategoryId = request.CategoryId;
        product.SellingPrice = request.SellingPrice;
        product.MRP = request.MRP;
        product.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(product.Id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(
    int id,
    CancellationToken cancellationToken)
{
    var product = await _context.Products
        .FirstOrDefaultAsync(
            p => p.Id == id,
            cancellationToken);

    if (product is null)
    {
        return false;
    }

    product.IsActive = false;

    await _context.SaveChangesAsync(cancellationToken);

    return true;
}
}