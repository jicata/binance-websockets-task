using System.Net.WebSockets;
using BinanceWebSocketTask.Business.Services.Contracts;

namespace BinanceWebSocketTask.Business.Services.Websockets.Contracts;

public interface IWebSocketAdapter : IService
{
    Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

    Task<WebSocketReceiveResult> ReceiveAsync(byte[] buffer, CancellationToken cancellationToken);

    Task SendDataAsync(
        byte[] buffer, WebSocketMessageType messageType,
        bool endOfMessage,
        CancellationToken cancellationToken);
    
    WebSocketState State { get; }

    void AbortAndDispose();
}