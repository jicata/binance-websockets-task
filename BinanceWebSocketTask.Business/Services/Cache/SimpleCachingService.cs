using System.Collections.Concurrent;
using BinanceWebSocketTask.Business.Services.Cache.Models;
using BinanceWebSocketTask.Data.Entities.Enums;
using Newtonsoft.Json;

namespace BinanceWebSocketTask.Business.Services.Cache;

public class SimpleCachingService : ISimpleCachingService
{
    private ConcurrentDictionary<string, string> _cache = new ();

    public void AddItem<T>(T item) where T : ICachableItem, new()
    {
        var itemAsJson = JsonConvert.SerializeObject(item);
        if (_cache.ContainsKey(item.Id))
        {
            _cache.AddOrUpdate(item.Id, itemAsJson, (key, oldValue) => itemAsJson);
        }
        else
        {
            while (!_cache.TryAdd(item.Id, itemAsJson))
            {
                _cache.TryAdd(item.Id, itemAsJson);
            }
        }
    }

    public T GetItem<T>(string key) where T : ICachableItem, new()
    {
        var item = string.Empty;

        _cache.TryGetValue(key, out item);

        if (string.IsNullOrEmpty(item))
        {
            return default;
        }
            
        return JsonConvert.DeserializeObject<T>(item);
    }

    public string BuildKeyForItem(string id, SimpleMovingAverageDataPointTimeInterval timeInterval)
        => $"{id}_{timeInterval}";
}