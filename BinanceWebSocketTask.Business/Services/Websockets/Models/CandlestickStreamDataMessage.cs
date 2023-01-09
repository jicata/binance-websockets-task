using Newtonsoft.Json;

namespace BinanceWebSocketTask.Business.Services.Websockets.Models;

public class CandlestickStreamDataMessage
{
    [JsonProperty("E")]
    public string EventTime { get; set; }
    
    [JsonProperty("s")]
    public string Symbol { get; set; }

    [JsonProperty("k")]
    public CandlestickData CandlestickData { get; set; }
}