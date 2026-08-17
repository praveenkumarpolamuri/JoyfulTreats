using JoyfulTreats.Application.DTOs.Purchases;

namespace JoyfulTreats.Application.Services.Purchases;

public interface IPurchaseService
{
    Task<IReadOnlyList<PurchaseDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<PurchaseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<PurchaseDto> CreateAsync(
        CreatePurchaseDto request,
        CancellationToken cancellationToken);

    Task<PurchaseDto?> UpdateAsync(
        int id,
        UpdatePurchaseDto request,
        CancellationToken cancellationToken);

    Task<PurchaseDto?> ReceiveAsync(
        int id,
        CancellationToken cancellationToken);

    Task<bool> CancelAsync(
        int id,
        CancellationToken cancellationToken);
}