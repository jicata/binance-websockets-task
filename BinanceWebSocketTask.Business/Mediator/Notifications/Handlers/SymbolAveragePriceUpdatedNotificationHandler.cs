using BinanceWebSocketTask.Business.Services.Cache;
using BinanceWebSocketTask.Business.Services.Cache.Models;
using BinanceWebSocketTask.Business.Services.Contracts;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Notifications.Handlers;

public class SymbolAveragePriceUpdatedNotificationHandler : INotificationHandler<SymbolAveragePriceUpdatedNotification>
{
    private readonly ISimpleCachingService _cache;

    public SymbolAveragePriceUpdatedNotificationHandler(ISimpleCachingService cache)
    {
        _cache = cache;
    }

    public Task Handle(SymbolAveragePriceUpdatedNotification notification, CancellationToken cancellationToken)
    {
        this._cache.AddItem(new SymbolAveragePriceCacheItem
        {
            AveragePrice = notification.AveragePrice,
            Id = notification.Symbol,
            AveragePriceTimeSpanInMinutes = TimeSpan.FromMinutes(notification.AveragePriceTimeSpanInMinutes)
        });
        
        return Task.CompletedTask;
    }
}