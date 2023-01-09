using Microsoft.Extensions.DependencyInjection;

using BinanceWebSocketTask.Business.Mediator.Queries;
using BinanceWebSocketTask.Business.Models;
using BinanceWebSocketTask.Business.Services.Cache;
using BinanceWebSocketTask.Business.Services.Cache.Models;
using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Data.Data;
using BinanceWebSocketTask.Data.Entities;

namespace BinanceWebSocketTask.Business.Services;

public class SymbolAveragePriceService : ISymbolAveragePriceService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISimpleCachingService _cache;

    public SymbolAveragePriceService(IServiceScopeFactory scopeFactory, ISimpleCachingService caching)
    {
        _scopeFactory = scopeFactory;
        _cache = caching;
    }

    public async Task<SymbolAveragePrice> CreateSymbolAveragePrice(string symbol, double averagePrice)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BinanceWebSocketTaskDbContext>();
        
        var averagePriceEntity = await context.SymbolAveragePrices.FindAsync(symbol);

        if (averagePriceEntity == null)
        {
            averagePriceEntity = new SymbolAveragePrice
            {
                Symbol = symbol,
                PreviousAveragePrices = new List<double> {averagePrice},
                AveragePrice = averagePrice,
                AveragePriceTimeSpanInMinutes = 1
            };
                
            await context.AddAsync(averagePriceEntity);
        }
        else
        {
            // since we only care for the last 24 hours
            // check if we have more records than necessary to cover the last 24 hours
            // if so, remove the oldest record and don't increment the average timespan, since we will always 
            // be using the 24 hour span from here on out
            if (averagePriceEntity.PreviousAveragePrices.Count >= 60 * 24) // 60 minutes times 24 hours
            {
                averagePriceEntity.PreviousAveragePrices.RemoveAt(0);
            }
            else
            {
                averagePriceEntity.AveragePriceTimeSpanInMinutes++;
            }
              
            averagePriceEntity.PreviousAveragePrices.Add(averagePrice);
                
               
            averagePriceEntity.AveragePrice = averagePriceEntity.PreviousAveragePrices.Average();

            context.Update(averagePriceEntity);
        }

        await context.SaveChangesAsync();

        return averagePriceEntity;
    }

    public async Task<Symbol24hAveragePriceModel> GetSymbol24hAveragePrice(GetSymbol24hAveragePriceQuery request)
    {
        var cachedItem = this._cache.GetItem<SymbolAveragePriceCacheItem>(request.Symbol.ToUpper());

        if (cachedItem == null)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BinanceWebSocketTaskDbContext>();
            var item = await context.SymbolAveragePrices.FindAsync(request.Symbol.ToUpper());

            if (item == null)
            {
                return new Symbol24hAveragePriceModel
                {
                    Message =
                        $"No records for symbol \"{request.Symbol}\" yet. Make sure the symbol is supported and try again in a minute"
                };
            }
            
            return new Symbol24hAveragePriceModel
            {
                Symbol = item.Symbol,
                AveragePrice = item.AveragePrice,
                TimeSpanOfAverage = TimeSpan.FromMinutes(item.AveragePriceTimeSpanInMinutes).ToString()
            };
        }
        
        return new Symbol24hAveragePriceModel
        {
            Symbol = cachedItem.Id,
            AveragePrice = cachedItem.AveragePrice,
            TimeSpanOfAverage = cachedItem.AveragePriceTimeSpanInMinutes.ToString()
        };
    }
}