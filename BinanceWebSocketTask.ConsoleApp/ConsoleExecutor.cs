using System.Text;
using BinanceWebSocketTask.Business.Mediator.Queries;
using MediatR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace BinanceWebSocketTask.ConsoleApp;

public class ConsoleService : IHostedService
{
    private const string TwentyFourHourAveragePriceCommandBeginning = "24h";
    private const string SimpleMovingAveragePriceCommamndBeginning = "sma";

    private readonly IMediator _mediator;
    private readonly IHostApplicationLifetime _lifetime;

    public ConsoleService(IMediator mediator,IHostApplicationLifetime lifetime)
    {
        _mediator = mediator;
        _lifetime = lifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var stringBuilder = new StringBuilder();
            
        stringBuilder.AppendLine("==================================================================");
        stringBuilder.AppendLine("Welcome to BinanceWebSocketTask.WebApi Console app!");
        stringBuilder.AppendLine("Allowed commands are: ");
        stringBuilder.AppendLine("- 24h {symbol}");
        stringBuilder.AppendLine("- sma {symbol} {n} {p} {s}");
        stringBuilder.AppendLine("To exit the application simply enter an empty command. Have fun!");
        stringBuilder.AppendLine("==================================================================");
            
        Console.WriteLine(stringBuilder.ToString());

        var command = Console.ReadLine();

        while (!string.IsNullOrEmpty(command))
        {
            Console.WriteLine(await HandleCommand(command));
            command = Console.ReadLine();
        }

        Console.WriteLine("Bye!");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Run(() => _lifetime.StopApplication());
    }
    
    
    private async Task<string> HandleCommand(string command)
    {
        if (command.StartsWith(TwentyFourHourAveragePriceCommandBeginning))
        {
            return await this.Handle24hAveragePriceCommand(command);
        }
        else if (command.StartsWith(SimpleMovingAveragePriceCommamndBeginning))
        {
            return await this.HandleSimpleMovingAveragePriceCommamnd(command);
        }
        
        return "Unknown command!";
    }

    private async Task<string> HandleSimpleMovingAveragePriceCommamnd(string command)
    {
        var commandArguments = command.Split(" ");
        var symbol = commandArguments[1];
        var numberOfDataPoints = int.Parse(commandArguments[2]);
        var dataPointsTimeInterval = commandArguments[3];
        var dataPointsEndDate = string.Empty;

        if (commandArguments.Length == 5)
        {
            dataPointsEndDate = commandArguments[4];
        }
        
        return JsonConvert.SerializeObject(await this._mediator.Send(new GetSymbolSimpleMovingAveragePriceQuery
        {
            Symbol = symbol,
            DataPointsTimeInterval = dataPointsTimeInterval,
            NumberOfDataPoints = numberOfDataPoints,
            DataPointsEndDate = dataPointsEndDate
        }));
    }

    private async Task<string> Handle24hAveragePriceCommand(string command)
    {
        var commandArguments = command.Split(" ");
        var symbol = commandArguments[1];

        return JsonConvert.SerializeObject(await this._mediator.Send(new GetSymbol24hAveragePriceQuery
            { Symbol = symbol }));
    }
}