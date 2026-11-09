using AIS.Core;

namespace AIS.Infrastructure;

/// <summary>
/// Репозиторий в памяти с демонстрационными данными. Используется, пока не подключена
/// база данных PostgreSQL (см. VehicleRepository).
/// </summary>
public class InMemoryVehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _vehicles;
    private readonly List<Booking> _bookings;

    public InMemoryVehicleRepository()
    {
        _vehicles = new List<Vehicle>
        {
            Car("00000000-0000-0000-0000-000000000001", "XTA210930Y2765432", "А123ВС178", "Lada Vesta", VehicleClass.Economy, 63),
            Car("00000000-0000-0000-0000-000000000002", "XW7BF3FK40S123456", "В456ОР178", "Toyota Camry", VehicleClass.Business, 81),
            Car("00000000-0000-0000-0000-000000000003", "Z94CB41AAGR234567", "Е789КХ198", "Hyundai Solaris", VehicleClass.Economy, 47),
            Car("00000000-0000-0000-0000-000000000004", "WDB9066331S345678", "К321МН178", "Mercedes Sprinter", VehicleClass.Van, 92),
            Car("00000000-0000-0000-0000-000000000005", "XTAGAB110N0456789", "М654ТУ198", "Lada Granta", VehicleClass.Economy, 28),
            Car("00000000-0000-0000-0000-000000000006", "WBA5A5C50FD567890", "Н987АВ178", "BMW 520d", VehicleClass.Business, 74),
            Car("00000000-0000-0000-0000-000000000007", "Z8NAJL00050678901", "Р246СЕ198", "Kia Rio", VehicleClass.Economy, 55),
            Car("00000000-0000-0000-0000-000000000008", "XUFFE1030K0789012", "С135ХА178", "Ford Transit", VehicleClass.Van, 66)
        };
        _vehicles[6].Status = VehicleStatus.Maintenance;

        var day = DateTime.UtcNow.Date.AddDays(1);
        _bookings = new List<Booking>
        {
            Reserve(_vehicles[0], day.AddHours(9), day.AddHours(13)),
            Reserve(_vehicles[2], day.AddHours(10), day.AddHours(16)),
            Reserve(_vehicles[3], day.AddHours(8), day.AddHours(20)),
            Reserve(_vehicles[5], day.AddDays(1).AddHours(9), day.AddDays(1).AddHours(18))
        };
    }

    public Task<IReadOnlyList<Vehicle>> FindAvailableAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        var busy = _bookings.Where(b => b.Overlaps(fromUtc, toUtc)).Select(b => b.VehicleId).ToHashSet();
        IReadOnlyList<Vehicle> result = _vehicles
            .Where(v => v.Status != VehicleStatus.Maintenance && !busy.Contains(v.Id))
            .ToList();
        return Task.FromResult(result);
    }

    private static Vehicle Car(string id, string vin, string plate, string model, VehicleClass cls, double fuel)
    {
        return new Vehicle
        {
            Id = Guid.Parse(id),
            Vin = vin,
            PlateNumber = plate,
            Model = model,
            VehicleClass = cls,
            Status = VehicleStatus.Available,
            FuelLevelPct = fuel
        };
    }

    private static Booking Reserve(Vehicle vehicle, DateTime startUtc, DateTime endUtc)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            EmployeeNumber = "004512",
            StartUtc = startUtc,
            EndUtc = endUtc
        };
    }
}
