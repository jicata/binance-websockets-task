using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using BusinessInitializer = BinanceWebSocketTask.Business.Initializer;
using DataInitializer = BinanceWebSocketTask.Data.Initializer;

namespace BinanceWebSocketTask.ConsoleApp
{
    public class Program
    {
        private static BusinessInitializer businessInitializer;
        private static DataInitializer dataInitializer;
        
        public static async Task Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            
            var host = CreateHostBuilder(args)
                .Build();

            var hostScope = host.Services.CreateScope();
            await dataInitializer.ConfigureServicesOnStartup(hostScope);

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configuration =>
                {
                    configuration
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hostBuilder, services) =>
                {
                    businessInitializer = new BusinessInitializer(hostBuilder.Configuration);
                    dataInitializer = new DataInitializer(hostBuilder.Configuration);

                    businessInitializer.ConfigureService(services);
                    dataInitializer.ConfigureService(services);
                    
                    services.AddHostedService<ConsoleService>();
                });
    }
}

