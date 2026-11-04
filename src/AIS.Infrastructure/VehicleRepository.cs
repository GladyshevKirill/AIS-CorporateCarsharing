using AIS.Core;
using Microsoft.EntityFrameworkCore;

namespace AIS.Infrastructure;

/// <summary>Реализация IVehicleRepository поверх EF Core.</summary>
public class VehicleRepository : IVehicleRepository
{
    private readonly CarsharingDbContext _db;

    public VehicleRepository(CarsharingDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Vehicle>> FindAvailableAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        var busyIds = _db.Bookings
            .Where(b => b.StartUtc < toUtc && fromUtc < b.EndUtc)
            .Select(b => b.VehicleId);

        return await _db.Vehicles
            .Where(v => v.Status != VehicleStatus.Maintenance && !busyIds.Contains(v.Id))
            .ToListAsync(ct);
    }
}
