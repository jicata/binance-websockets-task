using BinanceWebSocketTask.Business.Models;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Queries;

public class GetSymbol24hAveragePriceQuery : IRequest<Symbol24hAveragePriceModel>
{
    public string Symbol { get; set; }
}