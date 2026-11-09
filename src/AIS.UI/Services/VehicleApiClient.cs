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
        if (!response.IsSuccessStatusCode)
        {
            var error = await TryReadErrorAsync(response, ct);
            throw new HttpRequestException(error is null
                ? $"Ошибка сервера: {(int)response.StatusCode}"
                : $"{(int)response.StatusCode} {error.Code}: {error.Message}");
        }

        var result = await response.Content.ReadFromJsonAsync<VehiclePageDto>(cancellationToken: ct);
        return result ?? throw new InvalidOperationException("Пустой ответ сервера");
    }

    private static async Task<ErrorDto?> TryReadErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<ErrorDto>(cancellationToken: ct);
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }
}
