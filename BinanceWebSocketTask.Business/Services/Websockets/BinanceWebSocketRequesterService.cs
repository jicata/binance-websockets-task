using System.Net.WebSockets;
using System.Text;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using BinanceWebSocketTask.Business.Configuration;
using BinanceWebSocketTask.Business.Services.Websockets.Contracts;
using BinanceWebSocketTask.Business.Services.Websockets.Models;

using static BinanceWebSocketTask.Business.Constants.BinanceAPICommunicationConstants;

namespace BinanceWebSocketTask.Business.Services.Websockets;

public class BinanceWebSocketRequesterService : IBinanceWebSocketRequesterService
{
    private IWebSocketAdapter _webSocket;

    private readonly BinanceStreamsConfiguration _binanceStreamsConfiguration;

    public BinanceWebSocketRequesterService(IOptions<BinanceStreamsConfiguration> binanceStreamsConfiguration)
    {
        _binanceStreamsConfiguration = binanceStreamsConfiguration.Value;
    }

    public IWebSocketAdapter WebSocket => _webSocket;
    
    public async Task EstablishConnection()
    {
        try
        {
            while (this._webSocket is not { State: WebSocketState.Open })
            {
                await this.SubscribeToStreams();
            
                if (this._webSocket is { State: WebSocketState.Open })
                {
                    break;
                }
            
                await Task.Delay(this._binanceStreamsConfiguration.TimeIntervalBetweenConnectionAttemptsMilliseconds);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Connection attempt failed with {e}");
        }
        
    }

    private async Task SubscribeToStreams()
    {
        if (this._webSocket != null)
        {
            this._webSocket.AbortAndDispose();
        }
        var webSocket = new WebSocketAdapter();
        var uri = new Uri(
            $"{_binanceStreamsConfiguration.BaseEndpoint}" +
            $"{_binanceStreamsConfiguration.WebSocketStreamUriFragment}" +
            $"{_binanceStreamsConfiguration.WebSocketMultipleStreamsQueryParameter}{string.Join("/", this._binanceStreamsConfiguration.Symbols)}");
        await webSocket.ConnectAsync(uri, CancellationToken.None);
        
        foreach (var symbol in this._binanceStreamsConfiguration.Symbols)
        {
            var subscribeRequest = new StreamRequest<uint>
            {
                Id = (uint)new Random().Next(0, 1000),
                Method = SUBSCRIBE_METHOD_NAME,
                Params = new[] { $"{symbol}{KLINE_STREAM_HANDLE}" }
            };
        
            await this.SendWebSocketRequest(subscribeRequest, webSocket);
        }

        this._webSocket = webSocket;
    }

    private async Task SendWebSocketRequest<T>(StreamRequest<T> request, IWebSocketAdapter websocket)
    {
        var jsonRequest = JsonConvert.SerializeObject(request);
        
        await websocket.SendDataAsync(
            Encoding.Default.GetBytes(jsonRequest),
            WebSocketMessageType.Text,
            true, 
            CancellationToken.None);
    }
}