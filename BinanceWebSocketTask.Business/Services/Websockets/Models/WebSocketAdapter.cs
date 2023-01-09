using System.Net.WebSockets;
using BinanceWebSocketTask.Business.Services.Websockets.Contracts;

namespace BinanceWebSocketTask.Business.Services.Websockets.Models;

public class WebSocketAdapter : IWebSocketAdapter
{
    private readonly ClientWebSocket _websocket;

    public WebSocketAdapter()
    {
        _websocket = new ClientWebSocket();
    }
    
    public WebSocketState State  => this._websocket.State;

    public void AbortAndDispose()
    {
        this._websocket.Abort();
        this._websocket.Dispose();
    }
    
    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        => await this._websocket.ConnectAsync(uri, cancellationToken);

    public async Task<WebSocketReceiveResult> ReceiveAsync(byte[] buffer, CancellationToken cancellationToken)
        => await _websocket.ReceiveAsync(buffer, cancellationToken);

    public async Task SendDataAsync(
        byte[] buffer, WebSocketMessageType messageType,
        bool endOfMessage,
        CancellationToken cancellationToken) =>
        await _websocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

}