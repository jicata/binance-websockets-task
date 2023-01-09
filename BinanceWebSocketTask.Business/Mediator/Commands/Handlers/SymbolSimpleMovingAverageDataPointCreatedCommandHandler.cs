using BinanceWebSocketTask.Business.Services.Cache;
using BinanceWebSocketTask.Business.Services.Cache.Models;

using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Commands.Handlers;

public class SymbolSimpleMovingAverageDataPointCreatedCommandHandler 
    : IRequestHandler<SymbolSimpleMovingAverageDataPointCreatedCommand, Unit>
{
    private readonly ISimpleCachingService _cache;

    public SymbolSimpleMovingAverageDataPointCreatedCommandHandler(ISimpleCachingService cache)
        => _cache = cache;
    
    public Task<Unit> Handle(SymbolSimpleMovingAverageDataPointCreatedCommand request, CancellationToken cancellationToken)
    {
        var cacheItemId = this._cache.BuildKeyForItem(request.Symbol, request.TimeInterval);
        
        var simpleMovingAverageDataPointCacheItem = new SymbolSimpleMovingAverageDataPointCacheItem
        {
            Id = cacheItemId,
            DataPoints = new List<SymbolSimpleMovingAverageDataPoint>()
            {
                new ()
                {
                    AveragePrice = request.AveragePrice,
                    CreatedOn = request.CreatedOn,
                }
            }
        };

        var cachedItem = this._cache.GetItem<SymbolSimpleMovingAverageDataPointCacheItem>(cacheItemId);
        
        if (cachedItem == null)
        {
            this._cache.AddItem(simpleMovingAverageDataPointCacheItem);
        }
        else
        {
            cachedItem.DataPoints.AddRange(simpleMovingAverageDataPointCacheItem.DataPoints);
            this._cache.AddItem(cachedItem);
        }

        return Unit.Task;
    }
}