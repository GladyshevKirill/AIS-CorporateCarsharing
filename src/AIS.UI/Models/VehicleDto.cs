namespace AIS.UI.Models;

/// <summary>Автомобиль в ответе GET /api/v1/vehicles.</summary>
public record VehicleDto(
    Guid Id,
    string Vin,
    string PlateNumber,
    string Model,
    string VehicleClass,
    string Status,
    double FuelLevelPct);

/// <summary>Страница результатов поиска.</summary>
public record VehiclePageDto(IReadOnlyList<VehicleDto> Items, int Page, int Size, int Total);
