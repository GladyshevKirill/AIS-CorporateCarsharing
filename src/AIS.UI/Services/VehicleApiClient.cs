using System.Net.Http;
using System.Net.Http.Json;
using AIS.UI.Models;

namespace AIS.UI.Services;

/// <summary>Реализация клиента на HttpClient.</summary>
public class VehicleApiClient : IVehicleApiClient
{
    private readonly HttpClient _http;

    public VehicleApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<VehiclePageDto> SearchAsync(
        DateTime fromUtc,
        DateTime toUtc,
        string? vehicleClass,
        int page,
        int size,
        CancellationToken ct = default)
    {
        var url = $"/api/v1/vehicles?from={Uri.EscapeDataString(fromUtc.ToString("O"))}" +
                  $"&to={Uri.EscapeDataString(toUtc.ToString("O"))}&page={page}&size={size}";
        if (!string.IsNullOrEmpty(vehicleClass))
        {
            url += $"&class={Uri.EscapeDataString(vehicleClass)}";
        }

        using var response = await _http.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<VehiclePageDto>(cancellationToken: ct);
        return result ?? throw new InvalidOperationException("Пустой ответ сервера");
    }
}
