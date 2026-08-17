using JoyfulTreats.Application.DTOs.Products;

namespace JoyfulTreats.Application.Services.Products;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<ProductDto> CreateAsync(
        CreateProductDto request,
        CancellationToken cancellationToken);

        Task<ProductDto?> UpdateAsync(
        int id,
        UpdateProductDto request,           
    CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}