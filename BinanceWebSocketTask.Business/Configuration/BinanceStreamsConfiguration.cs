namespace BinanceWebSocketTask.Business.Configuration;

public record BinanceStreamsConfiguration
{
    public string BaseEndpoint { get; init; }

    public string WebSocketStreamUriFragment { get; init; }

    public string WebSocketMultipleStreamsQueryParameter { get; set; }
    
    public int TimeIntervalBetweenConnectionAttemptsMilliseconds { get; set; }
    
    public string[] Symbols { get; init; }
}