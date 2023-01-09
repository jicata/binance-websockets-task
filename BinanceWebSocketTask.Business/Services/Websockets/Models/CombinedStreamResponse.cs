using Newtonsoft.Json;

namespace BinanceWebSocketTask.Business.Services.Websockets.Models;

public class CombinedStreamResponse<T> where T : class, new()
{
    [JsonProperty("stream")]
    public string Stream { get; set; }
    
    [JsonProperty("data")]
    public T Data { get; set; }
}