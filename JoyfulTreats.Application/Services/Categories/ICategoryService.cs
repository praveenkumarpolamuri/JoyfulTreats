using JoyfulTreats.Application.DTOs.Categories;

namespace JoyfulTreats.Application.Services.Categories;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<CategoryDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<CategoryDto> CreateAsync(
        CreateCategoryDto request,
        CancellationToken cancellationToken);
        Task<CategoryDto?> UpdateAsync(
        int id,
        UpdateCategoryDto request,
        CancellationToken cancellationToken);
}