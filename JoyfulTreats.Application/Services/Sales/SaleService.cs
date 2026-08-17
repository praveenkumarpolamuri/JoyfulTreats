using JoyfulTreats.Application.DTOs.Sales;
using JoyfulTreats.Application.Interfaces.Persistence;
using JoyfulTreats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoyfulTreats.Application.Services.Sales;

public class SaleService(IApplicationDbContext context) : ISaleService
{
    public async Task<IReadOnlyList<SaleDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.Sales.AsNoTracking()
            .OrderByDescending(sale => sale.SaleDate)
            .ThenByDescending(sale => sale.Id)
            .Select(sale => new SaleDto
            {
                Id = sale.Id,
                ProductId = sale.ProductId,
                ProductName = sale.Product.Name,
                SaleDate = sale.SaleDate,
                Quantity = sale.Quantity,
                UnitPrice = sale.UnitPrice,
                TotalAmount = sale.Quantity * sale.UnitPrice
            })
            .ToListAsync(cancellationToken);

    public async Task<SaleDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await context.Sales.AsNoTracking()
            .Where(sale => sale.Id == id)
            .Select(sale => new SaleDto
            {
                Id = sale.Id,
                ProductId = sale.ProductId,
                ProductName = sale.Product.Name,
                SaleDate = sale.SaleDate,
                Quantity = sale.Quantity,
                UnitPrice = sale.UnitPrice,
                TotalAmount = sale.Quantity * sale.UnitPrice
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<SaleDto> CreateAsync(CreateSaleDto request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, cancellationToken);
        var sale = new Sale
        {
            ProductId = request.ProductId,
            SaleDate = request.SaleDate,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            CreatedAt = DateTime.UtcNow
        };
        context.Sales.Add(sale);
        await context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(sale.Id, cancellationToken)
            ?? throw new InvalidOperationException("Sale could not be retrieved after creation.");
    }

    public async Task<SaleDto?> UpdateAsync(int id, UpdateSaleDto request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, cancellationToken);
        var sale = await context.Sales.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (sale is null) return null;

        sale.ProductId = request.ProductId;
        sale.SaleDate = request.SaleDate;
        sale.Quantity = request.Quantity;
        sale.UnitPrice = request.UnitPrice;
        sale.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var sale = await context.Sales.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (sale is null) return false;
        context.Sales.Remove(sale);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ValidateAsync(CreateSaleDto request, CancellationToken cancellationToken)
    {
        if (request.ProductId <= 0 || request.SaleDate == default || request.Quantity <= 0 || request.UnitPrice < 0)
            throw new ArgumentException("Product, sale date, positive quantity, and non-negative unit price are required.");

        var productExists = await context.Products.AnyAsync(product => product.Id == request.ProductId && product.IsActive, cancellationToken);
        if (!productExists) throw new InvalidOperationException("The specified active product does not exist.");
    }
}
