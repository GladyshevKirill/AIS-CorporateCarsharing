namespace AIS.Core;

/// <summary>Контракт доступа к данным об автомобилях (реализуется в AIS.Infrastructure).</summary>
public interface IVehicleRepository
{
    Task<IReadOnlyList<Vehicle>> FindAvailableAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
}
