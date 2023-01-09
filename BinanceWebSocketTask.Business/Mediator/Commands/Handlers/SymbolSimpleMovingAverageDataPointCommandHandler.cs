using BinanceWebSocketTask.Business.Mediator.Notifications;
using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Data.Entities;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Commands.Handlers;

public class SymbolSimpleMovingAverageDataPointCommandHandler : 
    IRequestHandler<SymbolSimpleMovingAverageDataPointCommand, SymbolSimpleMovingAveragePriceDataPoint>
{
    private readonly ISymbolSimpleMovingAverageDataPointService _symbolSimpleMovingAverageService;
    private readonly IMediator _mediator;


    public SymbolSimpleMovingAverageDataPointCommandHandler(
        IMediator mediator,
        ISymbolSimpleMovingAverageDataPointService symbolSimpleMovingAverageService)
    {
        _mediator = mediator;
        _symbolSimpleMovingAverageService = symbolSimpleMovingAverageService;
    }

    public async Task<SymbolSimpleMovingAveragePriceDataPoint> Handle(
        SymbolSimpleMovingAverageDataPointCommand request, 
        CancellationToken cancellationToken)
    {
        var simpleMovingAveragePrice = await this._symbolSimpleMovingAverageService.CreateSimpleMovingAverageDataPoints(
            request.Symbol,
            request.HighPrice,
            request.LowPrice);

        await this._mediator.Publish(new SymbolSimpleMovingAverageDataPointCreatedNotification
        {
            CreatedOn = simpleMovingAveragePrice.CreatedOn,
            AveragePrice = simpleMovingAveragePrice.AveragePrice,
            Symbol = request.Symbol
        });

        return simpleMovingAveragePrice;
    }
}