namespace AIS.Core;

/// <summary>Автомобиль корпоративного автопарка.</summary>
public class Vehicle
{
    public Guid Id { get; set; }
    public string Vin { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public VehicleClass VehicleClass { get; set; }
    public VehicleStatus Status { get; set; }
    public double FuelLevelPct { get; set; }
}
