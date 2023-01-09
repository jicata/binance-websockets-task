using BinanceWebSocketTask.Business.Mediator.Queries;
using BinanceWebSocketTask.Business.Models;
using BinanceWebSocketTask.Data.Entities;

namespace BinanceWebSocketTask.Business.Services.Contracts;

public interface ISymbolSimpleMovingAverageDataPointService : IService
{
    Task<SymbolSimpleMovingAveragePriceDataPoint> CreateSimpleMovingAverageDataPoints(string notificationSymbol,
        string notificationHighPrice,
        string notificationLowPrice);

    SymbolSimpleMovingAveragePriceModel GetSymbolSimpleMovingAverage(GetSymbolSimpleMovingAveragePriceQuery queryModel);
}