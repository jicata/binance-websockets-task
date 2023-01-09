using BinanceWebSocketTask.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceWebSocketTask.Data;

public class Initializer
{
    public IConfiguration Configuration { get; }
    public IServiceCollection Services { get; set; }
    
    public Initializer(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    public void ConfigureService(IServiceCollection services)
    {
        services
            .AddEntityFrameworkSqlite()
            .AddDbContext<BinanceWebSocketTaskDbContext>(opt =>
            {
                opt.UseSqlite("Filename=BinanceWebSocketsTask.db");
                //opt.UseSqlite("Filename=:memory:");
            });

        this.Services = services;
    }
    
    public async Task ConfigureServicesOnStartup(IServiceScope? serviceScope)
    {
        using (serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<BinanceWebSocketTaskDbContext>();
            await context.Configure();
        }
    }
}