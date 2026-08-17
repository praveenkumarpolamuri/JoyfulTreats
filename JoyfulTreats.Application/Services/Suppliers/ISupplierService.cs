using JoyfulTreats.Application.DTOs.Suppliers;

namespace JoyfulTreats.Application.Services.Suppliers;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<SupplierDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<SupplierDto> CreateAsync(CreateSupplierDto request, CancellationToken cancellationToken);

    Task<SupplierDto?> UpdateAsync(int id, UpdateSupplierDto request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
