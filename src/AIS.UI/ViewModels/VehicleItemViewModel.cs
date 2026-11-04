using AIS.UI.Models;

namespace AIS.UI.ViewModels;

/// <summary>Строка списка автомобилей.</summary>
public class VehicleItemViewModel
{
    public VehicleItemViewModel(VehicleDto dto)
    {
        Title = dto.Model;
        Plate = dto.PlateNumber;
        Status = dto.Status;
        Fuel = $"{dto.FuelLevelPct:0}%";
    }

    public string Title { get; }
    public string Plate { get; }
    public string Status { get; }
    public string Fuel { get; }
}
