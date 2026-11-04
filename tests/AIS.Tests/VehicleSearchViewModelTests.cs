using AIS.UI.Models;
using AIS.UI.Services;
using AIS.UI.ViewModels;
using Xunit;

namespace AIS.Tests;

public class VehicleSearchViewModelTests
{
    private sealed class FakeApiClient : IVehicleApiClient
    {
        public Task<VehiclePageDto> SearchAsync(
            DateTime fromUtc, DateTime toUtc, string? vehicleClass, int page, int size, CancellationToken ct = default)
        {
            var items = new List<VehicleDto>
            {
                new(Guid.NewGuid(), "XTA210930Y2765432", "А123ВС178", "Lada Vesta", "ECONOMY", "AVAILABLE", 63)
            };
            return Task.FromResult(new VehiclePageDto(items, 1, 50, 1));
        }
    }

    [Fact]
    public async Task SearchAsync_FillsVehiclesFromApi()
    {
        var vm = new VehicleSearchViewModel(new FakeApiClient());

        await vm.SearchAsync();

        Assert.Single(vm.Vehicles);
        Assert.Equal("Lada Vesta", vm.Vehicles[0].Title);
        Assert.False(vm.IsBusy);
        Assert.Null(vm.ErrorMessage);
    }
}
