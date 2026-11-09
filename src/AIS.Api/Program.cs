using AIS.Core;
using AIS.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Реализация GET /api/v1/vehicles по спецификации docs/openapi/openapi.yaml.
// Демонстрационная проверка токена: принимается любой заголовок Authorization: Bearer ...
app.MapGet("/api/v1/vehicles", async (
    HttpRequest request,
    IVehicleRepository repository,
    string? from,
    string? to,
    [FromQuery(Name = "class")] string? vehicleClass,
    int? page,
    int? size,
    CancellationToken ct) =>
{
    var authorization = request.Headers.Authorization.ToString();
    if (!authorization.StartsWith("Bearer ", StringComparison.Ordinal))
    {
        return Fail(StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "Токен отсутствует или недействителен");
    }

    if (!DateTime.TryParse(from, null, System.Globalization.DateTimeStyles.RoundtripKind, out var fromUtc) ||
        !DateTime.TryParse(to, null, System.Globalization.DateTimeStyles.RoundtripKind, out var toUtc))
    {
        return Fail(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Параметры from и to обязательны (RFC 3339)");
    }

    if (fromUtc >= toUtc)
    {
        return Fail(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Параметр to должен быть позже from");
    }

    var pageNumber = page ?? 1;
    var pageSize = size ?? 20;
    if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
    {
        return Fail(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "page от 1, size от 1 до 100");
    }

    VehicleClass? requestedClass = null;
    if (!string.IsNullOrEmpty(vehicleClass))
    {
        if (!Enum.TryParse<VehicleClass>(vehicleClass, true, out var parsed))
        {
            return Fail(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "class: ECONOMY, BUSINESS или VAN");
        }

        requestedClass = parsed;
    }

    var available = await repository.FindAvailableAsync(fromUtc.ToUniversalTime(), toUtc.ToUniversalTime(), ct);
    var filtered = available.Where(v => requestedClass == null || v.VehicleClass == requestedClass).ToList();
    var items = filtered
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(v => new VehicleResponse(
            v.Id, v.Vin, v.PlateNumber, v.Model,
            v.VehicleClass.ToString().ToUpperInvariant(),
            StatusToApi(v.Status),
            v.FuelLevelPct))
        .ToList();

    return Results.Ok(new VehiclePageResponse(items, pageNumber, pageSize, filtered.Count));
});

app.Run();

static IResult Fail(int statusCode, string code, string message)
{
    return Results.Json(new ErrorResponse("ERROR", code, message, Guid.NewGuid().ToString("N")), statusCode: statusCode);
}

static string StatusToApi(VehicleStatus status)
{
    return status switch
    {
        VehicleStatus.Available => "AVAILABLE",
        VehicleStatus.Booked => "BOOKED",
        VehicleStatus.InTrip => "IN_TRIP",
        _ => "MAINTENANCE"
    };
}

record VehicleResponse(Guid Id, string Vin, string PlateNumber, string Model, string VehicleClass, string Status, double FuelLevelPct);
record VehiclePageResponse(IReadOnlyList<VehicleResponse> Items, int Page, int Size, int Total);
record ErrorResponse(string Status, string Code, string Message, string TraceId);
