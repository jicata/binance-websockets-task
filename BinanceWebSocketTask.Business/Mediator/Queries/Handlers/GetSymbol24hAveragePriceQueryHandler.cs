using BinanceWebSocketTask.Business.Models;
using BinanceWebSocketTask.Business.Services.Contracts;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Queries.Handlers;

public class GetSymbol24HAveragePriceQueryHandler : 
    IRequestHandler<GetSymbol24hAveragePriceQuery, Symbol24hAveragePriceModel>
{
    private readonly ISymbolAveragePriceService _symbolAveragePriceService;
    
    public GetSymbol24HAveragePriceQueryHandler( ISymbolAveragePriceService symbolAveragePriceService) 
        => _symbolAveragePriceService = symbolAveragePriceService;

    public Task<Symbol24hAveragePriceModel> Handle(GetSymbol24hAveragePriceQuery request, CancellationToken cancellationToken)
    {
        return this._symbolAveragePriceService.GetSymbol24hAveragePrice(request);
    }
}