using AIS.UI.Models;

namespace AIS.UI.Services;

/// <summary>Клиент API шлюза для каталога автомобилей.</summary>
public interface IVehicleApiClient
{
    Task<VehiclePageDto> SearchAsync(
        DateTime fromUtc,
        DateTime toUtc,
        string? vehicleClass,
        int page,
        int size,
        CancellationToken ct = default);
}
