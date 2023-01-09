using System.ComponentModel.DataAnnotations;

namespace BinanceWebSocketTask.Data.Entities;

public class SymbolAveragePrice
{
    [Key]
    public string Symbol { get; set; }

    [Required]
    public double AveragePrice { get; set; }

    [Required]
    public int AveragePriceTimeSpanInMinutes { get; set; }
    
    public List<double> PreviousAveragePrices { get; set; }
}