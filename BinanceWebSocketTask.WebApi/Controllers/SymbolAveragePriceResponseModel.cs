namespace BinanceWebSocketTask.WebApi.Controllers;

public class SymbolAveragePriceResponseModel
{
    public string Symbol { get; set; }

    public double AveragePrice { get; set; }

    public string TimeInterval { get; set; }
}