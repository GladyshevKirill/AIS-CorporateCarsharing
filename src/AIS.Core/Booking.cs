namespace AIS.Core;

/// <summary>Бронь автомобиля сотрудником на интервал времени.</summary>
public class Booking
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    /// <summary>Пересекается ли бронь с заданным интервалом (границы не включаются).</summary>
    public bool Overlaps(DateTime fromUtc, DateTime toUtc)
    {
        return StartUtc < toUtc && fromUtc < EndUtc;
    }
}
