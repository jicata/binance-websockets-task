using System.Net.WebSockets;
using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Business.Services.Websockets.Models;

namespace BinanceWebSocketTask.Business.Services.Websockets.Contracts;

public interface IBinanceWebSocketRequesterService : IService
{
    IWebSocketAdapter WebSocket { get; }

    Task EstablishConnection();
}