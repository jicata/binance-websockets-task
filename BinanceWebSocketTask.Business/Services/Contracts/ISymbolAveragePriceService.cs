using BinanceWebSocketTask.Business.Mediator.Queries;
using BinanceWebSocketTask.Business.Models;
using BinanceWebSocketTask.Data.Entities;

namespace BinanceWebSocketTask.Business.Services.Contracts;

public interface ISymbolAveragePriceService : IService
{
    Task<SymbolAveragePrice> CreateSymbolAveragePrice(string symbol, double averagePrice);
    
    Task<Symbol24hAveragePriceModel> GetSymbol24hAveragePrice(GetSymbol24hAveragePriceQuery request);
}