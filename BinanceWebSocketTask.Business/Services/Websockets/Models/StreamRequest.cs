using Newtonsoft.Json;

namespace BinanceWebSocketTask.Business.Services.Websockets.Models;

public class StreamRequest<T>
{
    [JsonProperty("id")]
    public T Id { get; set; }
    
    [JsonProperty("method")]
    public string Method { get; set; }
    
    [JsonProperty("params")]
    public string[] Params { get; set; }
}