using System.ComponentModel.DataAnnotations;
using BinanceWebSocketTask.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace BinanceWebSocketTask.Data.Entities;

[Index(nameof(TimeInterval))]
public class SymbolSimpleMovingAveragePriceDataPoint
{
    [Key]
    public DateTime CreatedOn { get; set; } 

    public string Symbol { get; set; }
    
    public SimpleMovingAverageDataPointTimeInterval TimeInterval { get; set; }

    public double AveragePrice { get; set; }
}