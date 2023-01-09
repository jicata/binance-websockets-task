using BinanceWebSocketTask.Data.Entities;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Commands;

public class SymbolSimpleMovingAverageDataPointCommand : IRequest<SymbolSimpleMovingAveragePriceDataPoint>
{
    public string Symbol { get; set; }

    public string HighPrice { get; set; }

    public string LowPrice { get; set; }
}