using BinanceWebSocketTask.Business.Models;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Queries;

public class GetSymbolSimpleMovingAveragePriceQuery : IRequest<SymbolSimpleMovingAveragePriceModel>
{
    public string Symbol { get; set; }

    public int NumberOfDataPoints { get; set; }
    
    public string DataPointsTimeInterval { get; set; }

    public string DataPointsEndDate { get; set; }
}