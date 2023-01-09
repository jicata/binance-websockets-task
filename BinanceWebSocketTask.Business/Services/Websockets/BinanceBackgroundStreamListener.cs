using System.Net.WebSockets;
using System.Text;
using BinanceWebSocketTask.Business.Configuration;
using BinanceWebSocketTask.Business.Mediator.Commands;
using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Business.Services.Websockets.Contracts;
using BinanceWebSocketTask.Business.Services.Websockets.Models;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BinanceWebSocketTask.Business.Services.Websockets;

public class BinanceBackgroundStreamListener : BackgroundService
{
    private readonly IBinanceWebSocketRequesterService _webSocketRequesterService;
    private readonly IMediator _mediator;

    public BinanceBackgroundStreamListener(
        IBinanceWebSocketRequesterService webSocketRequesterService, 
        IMediator mediator)
    {
        _webSocketRequesterService = webSocketRequesterService;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this._webSocketRequesterService.EstablishConnection();

        await this.ListenToCandleStickStream(
            this._webSocketRequesterService.WebSocket);
    }

    private async Task ListenToCandleStickStream(IWebSocketAdapter webSocket)
    {
        var dataBuffer = new byte[1024];

        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                await webSocket.ReceiveAsync(dataBuffer, CancellationToken.None);
                
                var streamMessage = JsonConvert.DeserializeObject<CombinedStreamResponse<CandlestickStreamDataMessage>>(
                    Encoding.Default.GetString(dataBuffer));

                if (streamMessage == null || streamMessage.Data == null)
                {
                    Console.WriteLine($"Control message received: {Encoding.Default.GetString(dataBuffer)}");
                }
                else if (streamMessage.Data.CandlestickData.CandleStickIsClosed)
                {
                    var createSymbolInformationPerMinuteCommand = new SymbolSimpleMovingAverageDataPointCommand
                    {
                        Symbol = streamMessage.Data.Symbol,
                        HighPrice = streamMessage.Data.CandlestickData.HighPrice,
                        LowPrice = streamMessage.Data.CandlestickData.LowPrice
                    };
                    await this._mediator.Send(createSymbolInformationPerMinuteCommand);
                }
                
                dataBuffer = new byte[1024];
            }
            catch (WebSocketException e)
            {
                Console.WriteLine($"Error in connection. Trying to reconnect");
                
                await this._webSocketRequesterService.EstablishConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while parsing message from steam. " +
                                  $"Message was {Encoding.Default.GetString(dataBuffer)}. " +
                                  $"Error: {e}");
            }
        }
    }
}