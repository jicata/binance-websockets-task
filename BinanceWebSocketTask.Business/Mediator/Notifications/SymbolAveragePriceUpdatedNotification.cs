using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Notifications;

public class SymbolAveragePriceUpdatedNotification : INotification
{
    public string Symbol { get; set; }

    public double AveragePrice { get; set; }

    public int AveragePriceTimeSpanInMinutes { get; set; }
}