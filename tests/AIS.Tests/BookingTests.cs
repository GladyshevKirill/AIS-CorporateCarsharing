using AIS.Core;
using Xunit;

namespace AIS.Tests;

public class BookingTests
{
    private static readonly DateTime T0 = new(2026, 11, 10, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Overlaps_ReturnsTrue_WhenIntervalsIntersect()
    {
        var booking = new Booking { StartUtc = T0, EndUtc = T0.AddHours(4) };
        Assert.True(booking.Overlaps(T0.AddHours(2), T0.AddHours(6)));
    }

    [Fact]
    public void Overlaps_ReturnsFalse_WhenIntervalsOnlyTouch()
    {
        var booking = new Booking { StartUtc = T0, EndUtc = T0.AddHours(4) };
        Assert.False(booking.Overlaps(T0.AddHours(4), T0.AddHours(6)));
    }
}
