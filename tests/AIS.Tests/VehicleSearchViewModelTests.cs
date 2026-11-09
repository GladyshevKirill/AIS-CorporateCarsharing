using AIS.UI.Models;
using AIS.UI.Services;
using AIS.UI.ViewModels;
using Xunit;

namespace AIS.Tests;

public class VehicleSearchViewModelTests
{
    private sealed class FakeApiClient : IVehicleApiClient
    {
        public string? LastClass { get; private set; }

        public Task<VehiclePageDto> SearchAsync(
            DateTime fromUtc, DateTime toUtc, string? vehicleClass, int page, int size, CancellationToken ct = default)
        {
            LastClass = vehicleClass;
            var items = new List<VehicleDto>
            {
                new(Guid.NewGuid(), "XTA210930Y2765432", "А123ВС178", "Lada Vesta", "ECONOMY", "AVAILABLE", 63)
            };
            return Task.FromResult(new VehiclePageDto(items, 1, 50, 1));
        }
    }

    private sealed class FailingApiClient : IVehicleApiClient
    {
        public Task<VehiclePageDto> SearchAsync(
            DateTime fromUtc, DateTime toUtc, string? vehicleClass, int page, int size, CancellationToken ct = default)
        {
            throw new HttpRequestException("сервер недоступен");
        }
    }

    [Fact]
    public async Task SearchAsync_FillsVehiclesFromApi()
    {
        var vm = new VehicleSearchViewModel(new FakeApiClient());

        await vm.SearchAsync();

        Assert.Single(vm.Vehicles);
        Assert.Equal("Lada Vesta", vm.Vehicles[0].Title);
        Assert.Equal("Свободен", vm.Vehicles[0].Status);
        Assert.False(vm.IsBusy);
        Assert.Null(vm.ErrorMessage);
    }

    [Fact]
    public async Task SearchAsync_PassesSelectedClassToApi()
    {
        var api = new FakeApiClient();
        var vm = new VehicleSearchViewModel(api) { SelectedClass = "VAN" };

        await vm.SearchAsync();

        Assert.Equal("VAN", api.LastClass);
    }

    [Fact]
    public async Task SearchAsync_ShowsErrorWhenServerUnavailable()
    {
        var vm = new VehicleSearchViewModel(new FailingApiClient());

        await vm.SearchAsync();

        Assert.Empty(vm.Vehicles);
        Assert.Contains("сервер недоступен", vm.ErrorMessage);
    }
}
