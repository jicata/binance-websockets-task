using BinanceWebSocketTask.Business.Services.Contracts;

namespace BinanceWebSocketTask.Business.Services.Cache.Models;

public class SymbolAveragePriceCacheItem : ICachableItem
{
    public string Id { get; set; }
    
    public double AveragePrice { get; set; }

    public TimeSpan AveragePriceTimeSpanInMinutes { get; set; }
}