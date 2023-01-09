using BinanceWebSocketTask.Business.Services.Cache.Models;
using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Data.Entities.Enums;

namespace BinanceWebSocketTask.Business.Services.Cache;

public interface ISimpleCachingService 
{
    void AddItem<T>(T item) where T : ICachableItem, new();

    T GetItem<T>(string key) where T : ICachableItem, new ();

    string BuildKeyForItem(string id, SimpleMovingAverageDataPointTimeInterval timeInterval);
}