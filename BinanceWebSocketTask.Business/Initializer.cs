using System.Reflection;
using BinanceWebSocketTask.Business.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using BinanceWebSocketTask.Business.Mediator.Commands;
using BinanceWebSocketTask.Business.Services;
using BinanceWebSocketTask.Business.Services.Cache;
using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Business.Services.Websockets;
using BinanceWebSocketTask.Business.Validation;

using FluentValidation;

using MediatR;
using Microsoft.Extensions.Options;

namespace BinanceWebSocketTask.Business;

public class Initializer
{
    public IConfiguration Configuration { get; }
    
    public Initializer(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    public void ConfigureService(IServiceCollection services)
    {
        services.AddOptions();
        services.AddSingleton<IOptions<BinanceStreamsConfiguration>>(_ => 
            Options.Create(Configuration
            .GetSection(nameof(BinanceStreamsConfiguration))
            .Get<BinanceStreamsConfiguration>()));

        services.AddMediatR(typeof(SymbolSimpleMovingAverageDataPointCommand).GetTypeInfo().Assembly);

        var assembly = Assembly.GetAssembly(typeof(IService));

        foreach (var serviceInterface in assembly.GetTypes().Where(t => t != typeof(IService) && t.IsInterface && t.IsAssignableTo(typeof(IService))))
        {
            var concreteService = assembly.GetExportedTypes().FirstOrDefault(t => t.IsClass && t.IsAssignableTo(serviceInterface));

            services.AddTransient(serviceInterface, concreteService);
        }

        services.AddValidatorsFromAssemblyContaining<GetSymbolSimpleAveragePriceQueryValidator>();
        services.AddSingleton<ISimpleCachingService, SimpleCachingService>();
        services.AddHostedService<BinanceBackgroundStreamListener>();
    }
}