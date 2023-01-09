using Newtonsoft.Json;

namespace BinanceWebSocketTask.Business.Services.Websockets.Models;

public class CandlestickData
{
    [JsonProperty("t")]
    public string StartTime { get; set; }

    [JsonProperty("T")]
    public string CloseTime { get; set; }

    [JsonProperty("c")]
    public string ClosePrice { get; set; }
    
    [JsonProperty("h")]
    public string HighPrice { get; set; }
    
    [JsonProperty("l")]
    public string LowPrice { get; set; }

    [JsonProperty("x")]
    public bool CandleStickIsClosed { get; set; }
}