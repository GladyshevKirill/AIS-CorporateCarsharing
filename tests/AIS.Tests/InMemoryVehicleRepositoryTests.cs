using AIS.Core;
using AIS.Infrastructure;
using Xunit;

namespace AIS.Tests;

public class InMemoryVehicleRepositoryTests
{
    [Fact]
    public async Task FindAvailableAsync_ExcludesBookedAndMaintenanceVehicles()
    {
        var repository = new InMemoryVehicleRepository();
        var day = DateTime.UtcNow.Date.AddDays(1);

        var result = await repository.FindAvailableAsync(day.AddHours(9), day.AddHours(18));

        Assert.DoesNotContain(result, v => v.PlateNumber == "А123ВС178");
        Assert.DoesNotContain(result, v => v.Status == VehicleStatus.Maintenance);
        Assert.Contains(result, v => v.PlateNumber == "В456ОР178");
    }
}
