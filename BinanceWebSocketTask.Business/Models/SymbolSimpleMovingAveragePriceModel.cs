namespace BinanceWebSocketTask.Business.Models;

public class SymbolSimpleMovingAveragePriceModel : BaseModel
{
    public string Symbol { get; set; }

    public double? AveragePrice { get; set; }
    
    public int? ExpectedDataPoints { get; set; }
    
    public int? AvailableDataPoints { get; set; }
}