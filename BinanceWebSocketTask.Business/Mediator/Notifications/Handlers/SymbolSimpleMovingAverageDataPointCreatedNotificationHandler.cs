using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Data.Entities;
using BinanceWebSocketTask.Data.Entities.Enums;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Notifications.Handlers;

public class SymbolSimpleMovingAverageDataPointCreatedNotificationHandler 
    : INotificationHandler<SymbolSimpleMovingAverageDataPointCreatedNotification>
{
    private readonly ISymbolAveragePriceService _symbolAveragePriceService;
    private readonly IMediator _mediator;

    public SymbolSimpleMovingAverageDataPointCreatedNotificationHandler(
        IMediator mediator,
        ISymbolAveragePriceService symbolAveragePriceService)
    {
        _mediator = mediator;
        _symbolAveragePriceService = symbolAveragePriceService;
    }

    public async Task Handle(SymbolSimpleMovingAverageDataPointCreatedNotification notification, CancellationToken cancellationToken)
    {
        var symbolAveragePriceEntity = await this._symbolAveragePriceService.CreateSymbolAveragePrice(
            notification.Symbol,
            notification.AveragePrice);
        
        await this._mediator.Publish(new SymbolAveragePriceUpdatedNotification
        {
            Symbol = notification.Symbol,
            AveragePrice = symbolAveragePriceEntity.AveragePrice,
            AveragePriceTimeSpanInMinutes = symbolAveragePriceEntity.AveragePriceTimeSpanInMinutes
        });
    }
}