using AIS.Core;
using Microsoft.EntityFrameworkCore;

namespace AIS.Infrastructure;

/// <summary>Контекст EF Core для PostgreSQL.</summary>
public class CarsharingDbContext : DbContext
{
    public CarsharingDbContext(DbContextOptions<CarsharingDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Booking> Bookings => Set<Booking>();
}
