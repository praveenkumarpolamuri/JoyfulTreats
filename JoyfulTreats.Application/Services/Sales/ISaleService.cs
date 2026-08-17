using JoyfulTreats.Application.DTOs.Sales;

namespace JoyfulTreats.Application.Services.Sales;

public interface ISaleService
{
    Task<IReadOnlyList<SaleDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<SaleDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<SaleDto> CreateAsync(CreateSaleDto request, CancellationToken cancellationToken);

    Task<SaleDto?> UpdateAsync(int id, UpdateSaleDto request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
