using BinanceWebSocketTask.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static BinanceWebSocketTask.Data.Constants;

namespace BinanceWebSocketTask.Data.Data;

public class BinanceWebSocketTaskDbContext : DbContext
{
    public BinanceWebSocketTaskDbContext(DbContextOptions<BinanceWebSocketTaskDbContext> 
        options):base(options)
    {

    }

    public DbSet<SymbolAveragePrice> SymbolAveragePrices { get; set; }

    public DbSet<SymbolSimpleMovingAveragePriceDataPoint> SymbolSimpleMovingAveragePriceDataPoints { get; set; }

    public async Task Configure()
    {
        await this.Database.EnsureDeletedAsync();
        await this.Database.MigrateAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SymbolAveragePrice>()
            .Property(e => e.PreviousAveragePrices)
            .HasConversion(
                to => JsonConvert.SerializeObject(to),
                from => JsonConvert.DeserializeObject<List<double>>(from));
        
        modelBuilder.Entity<SymbolSimpleMovingAveragePriceDataPoint>()
            .Property(e => e.TimeInterval)
            .HasConversion(
                to => TimeIntervalConversionMap.FirstOrDefault(v => v.Value == to).Key,
                from => TimeIntervalConversionMap[from]);
    }
}