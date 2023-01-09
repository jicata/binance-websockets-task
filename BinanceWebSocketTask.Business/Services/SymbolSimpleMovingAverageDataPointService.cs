using System.Globalization;

using Microsoft.Extensions.DependencyInjection;

using MediatR;

using BinanceWebSocketTask.Business.Mediator.Commands;
using BinanceWebSocketTask.Business.Mediator.Queries;
using BinanceWebSocketTask.Business.Models;
using BinanceWebSocketTask.Business.Services.Cache;
using BinanceWebSocketTask.Business.Services.Cache.Models;
using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Data.Data;
using BinanceWebSocketTask.Data.Entities;
using BinanceWebSocketTask.Data.Entities.Enums;

using static BinanceWebSocketTask.Business.Constants;

namespace BinanceWebSocketTask.Business.Services;

public class SymbolSimpleMovingAverageDataPointService : ISymbolSimpleMovingAverageDataPointService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateService _dateService;
    private readonly IMediator _mediator;
    private readonly ISimpleCachingService _cache;

    // Store sequential intervals and the time difference between them
    private readonly Dictionary<(SimpleMovingAverageDataPointTimeInterval timeInterval ,SimpleMovingAverageDataPointTimeInterval nextTimeInterval), int>
        sequentialTimeIntervalsAndDifferences = new()
        {
            {(SimpleMovingAverageDataPointTimeInterval.OneMinute, SimpleMovingAverageDataPointTimeInterval.FiveMinutes), 5},
            {(SimpleMovingAverageDataPointTimeInterval.FiveMinutes, SimpleMovingAverageDataPointTimeInterval.ThirtyMinutes), 6},
            {(SimpleMovingAverageDataPointTimeInterval.ThirtyMinutes, SimpleMovingAverageDataPointTimeInterval.OneDay), 48},
            {(SimpleMovingAverageDataPointTimeInterval.OneDay, SimpleMovingAverageDataPointTimeInterval.OneWeek), 7},
        };

    public SymbolSimpleMovingAverageDataPointService(
        IServiceScopeFactory serviceScopeFactory,
        IDateService dateService, 
        IMediator mediator, ISimpleCachingService cache)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dateService = dateService;
        _mediator = mediator;
        _cache = cache;
    }
    
    public SymbolSimpleMovingAveragePriceModel GetSymbolSimpleMovingAverage(GetSymbolSimpleMovingAveragePriceQuery queryModel)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetService<BinanceWebSocketTaskDbContext>();

        var dataPointsEndDate = !string.IsNullOrEmpty(queryModel.DataPointsEndDate)
            ? DateTime.ParseExact(
                queryModel.DataPointsEndDate,
                DATE_FORMAT_GET_SIMPLE_MOVING_AVERAGE_QUERY,
                CultureInfo.InvariantCulture)
            : this._dateService.GetUtcNow();

        var dataPointsInterval = TimeIntervalConversionMap[queryModel.DataPointsTimeInterval];
        
        var symbolSimpleMovingAveragesFromCache = this._cache.GetItem<SymbolSimpleMovingAverageDataPointCacheItem>(
                this._cache.BuildKeyForItem(queryModel.Symbol, dataPointsInterval))?.DataPoints
            .Where(dp => dp.CreatedOn <= dataPointsEndDate)
            .Take(queryModel.NumberOfDataPoints);
        
        if (symbolSimpleMovingAveragesFromCache != null && symbolSimpleMovingAveragesFromCache.Any())
        {
            return new SymbolSimpleMovingAveragePriceModel
            {
                AvailableDataPoints = symbolSimpleMovingAveragesFromCache.Count(),
                AveragePrice = symbolSimpleMovingAveragesFromCache.Average(ssma => ssma.AveragePrice),
                ExpectedDataPoints = queryModel.NumberOfDataPoints,
                Symbol = queryModel.Symbol
            };
        }

        var symbolSimpleMovingAverages = context.SymbolSimpleMovingAveragePriceDataPoints
            .Where(smap => smap.Symbol == queryModel.Symbol
               && smap.TimeInterval == dataPointsInterval
               && smap.CreatedOn <= dataPointsEndDate)
            .Take(queryModel.NumberOfDataPoints);

        if (!symbolSimpleMovingAverages.Any())
        {
            return new SymbolSimpleMovingAveragePriceModel
            {
                AvailableDataPoints = 0,
                ExpectedDataPoints = queryModel.NumberOfDataPoints,
                Symbol = queryModel.Symbol
            };
        }
        
        return new SymbolSimpleMovingAveragePriceModel
        {
            AvailableDataPoints = symbolSimpleMovingAverages.Count(),
            AveragePrice = symbolSimpleMovingAverages.Average(ssma => ssma.AveragePrice),
            ExpectedDataPoints = queryModel.NumberOfDataPoints,
            Symbol = queryModel.Symbol
        };
    }
    
    public async Task<SymbolSimpleMovingAveragePriceDataPoint> CreateSimpleMovingAverageDataPoints(string symbol, string highPrice,
        string lowPrice)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<BinanceWebSocketTaskDbContext>();
        
        var simpleMovingAverageDataPoint = await this.CreateSimpleMovingAverage(
            symbol,
            SimpleMovingAverageDataPointTimeInterval.OneMinute,
            () => (double.Parse(highPrice) + double.Parse(lowPrice)) / 2,
            context);

        foreach (var simpleMovingAveragesTimePointsAndDifference in sequentialTimeIntervalsAndDifferences)
        {
            await TryToCreateSimpleMovingAverageDataPointBasedOnPreviousDataPointsInterval(
                symbol,
                simpleMovingAveragesTimePointsAndDifference.Key.timeInterval,
                simpleMovingAveragesTimePointsAndDifference.Key.nextTimeInterval,
                simpleMovingAveragesTimePointsAndDifference.Value,
                context);
        }

        return simpleMovingAverageDataPoint;

    }
    private async Task TryToCreateSimpleMovingAverageDataPointBasedOnPreviousDataPointsInterval(
        string symbol,
        SimpleMovingAverageDataPointTimeInterval previousTimeInterval,
        SimpleMovingAverageDataPointTimeInterval nextTimeInterval,
        int differenceBetweenTimeIntervals,
        BinanceWebSocketTaskDbContext context)
    {  
        // find the count of previous interval data points
        // and skip (CurrentIntervalValueDataPoints.Count * (CurrentDataPointInterval * PreviousDataPointInterval)) items]
        // to make sure we avoid previously used interval points
        // ex:
        // PreviousDataPointInterval = 1m
        // CurrentDataPointInterval = 5m
        // PreviousIntervalDataPoints.Count = 5,
        // CurrentIntervalValueDataPoints = 0,
        // SkipStep = (CurrentDataPointInterval * PreviousDataPointInterval) = 5 
        // We find 5 previous data points and skip (CurrentIntervalValueDataPoints * SkipStep) = 5.Skip(5*0) = 5.Skip(0)
        // We end of with 5 previous interval data points
        var previousSimpleMovingAverageDataPoints = context.SymbolSimpleMovingAveragePriceDataPoints
            .AsQueryable()
            .Where(sipm => sipm.Symbol == symbol && sipm.TimeInterval == previousTimeInterval)
            .Skip(context.SymbolSimpleMovingAveragePriceDataPoints.Count(
                sipm => sipm.Symbol == symbol && sipm.TimeInterval == nextTimeInterval) * differenceBetweenTimeIntervals);

        // divide PreviousIntervalDataPoints.Count by (CurrentDataPointInterval * PreviousDataPointInterval)
        // PreviousDataPointInterval = 1m
        // CurrentDataPointInterval = 5m
        // divisor = 5
        // if the remainder is 0 then we have enough previous data points to construct the next data point
        if (previousSimpleMovingAverageDataPoints.Any() &&
            previousSimpleMovingAverageDataPoints.Count() % differenceBetweenTimeIntervals == default)
        {
            await this.CreateSimpleMovingAverage(
                symbol,
                nextTimeInterval,
                () => previousSimpleMovingAverageDataPoints.Select(ssma => ssma.AveragePrice).Average(),
                context);
        }
    }
    
    private async Task<SymbolSimpleMovingAveragePriceDataPoint> CreateSimpleMovingAverage(
        string symbol, 
        SimpleMovingAverageDataPointTimeInterval timeInterval,
        Func<double> averageCalculationFunction,
        BinanceWebSocketTaskDbContext context)
    {
        var symbolSimpleMovingAveragePriceDataPoint = new SymbolSimpleMovingAveragePriceDataPoint
        {
            Symbol = symbol,
            AveragePrice = averageCalculationFunction(),
            CreatedOn = this._dateService.GetUtcNow(),
            TimeInterval = timeInterval
        };
        
        context.SymbolSimpleMovingAveragePriceDataPoints.Add(symbolSimpleMovingAveragePriceDataPoint);

        await this._mediator.Send(new SymbolSimpleMovingAverageDataPointCreatedCommand
        {
            AveragePrice = symbolSimpleMovingAveragePriceDataPoint.AveragePrice,
            CreatedOn = symbolSimpleMovingAveragePriceDataPoint.CreatedOn,
            Symbol = symbolSimpleMovingAveragePriceDataPoint.Symbol,
            TimeInterval = symbolSimpleMovingAveragePriceDataPoint.TimeInterval
        });
        
        await context.SaveChangesAsync();

        return symbolSimpleMovingAveragePriceDataPoint;
    }
}