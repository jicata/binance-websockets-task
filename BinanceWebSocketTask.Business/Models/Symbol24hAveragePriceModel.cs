namespace BinanceWebSocketTask.Business.Models;

public class Symbol24hAveragePriceModel : BaseModel
{
    public string Symbol { get; set; }
    
    public double AveragePrice { get; set; }
    
    public string TimeSpanOfAverage { get; set; }
}