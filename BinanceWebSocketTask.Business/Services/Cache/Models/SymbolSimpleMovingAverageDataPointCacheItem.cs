using BinanceWebSocketTask.Data.Entities.Enums;

namespace BinanceWebSocketTask.Business.Services.Cache.Models;

public class SymbolSimpleMovingAverageDataPointCacheItem : ICachableItem
{
    public string Id { get; set; }

    public List<SymbolSimpleMovingAverageDataPoint> DataPoints { get; set; }
}

public class SymbolSimpleMovingAverageDataPoint
{
    public DateTime CreatedOn { get; set; } 
    
    public double AveragePrice { get; set; }
}