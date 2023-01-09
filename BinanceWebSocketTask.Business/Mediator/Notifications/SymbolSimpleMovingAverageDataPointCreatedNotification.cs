using BinanceWebSocketTask.Data.Entities.Enums;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Notifications;

public class SymbolSimpleMovingAverageDataPointCreatedNotification : INotification
{
    public DateTime CreatedOn { get; set; }
    
    public string Symbol { get; set; }

    public double AveragePrice { get; set; }
    
    public SimpleMovingAverageDataPointTimeInterval TimeInterval { get; set; }
}