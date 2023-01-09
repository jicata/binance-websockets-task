using System.Text.Json.Serialization;
using BinanceWebSocketTask.WebApi.Middleware;
using Newtonsoft.Json;

using BusinessInitializer = BinanceWebSocketTask.Business.Initializer;
using DataInitializer = BinanceWebSocketTask.Data.Initializer;


JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    NullValueHandling = NullValueHandling.Ignore
};

var builder = WebApplication.CreateBuilder(args);

var businessInitializer = new BusinessInitializer(builder.Configuration);
var dataInitializer = new DataInitializer(builder.Configuration);

businessInitializer.ConfigureService(builder.Services);
dataInitializer.ConfigureService(builder.Services);

builder.Services.AddControllers()
    .AddXmlSerializerFormatters()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseMiddleware<RequestTypeMiddleware>();

await dataInitializer.ConfigureServicesOnStartup(app.Services.GetService<IServiceScopeFactory>()?.CreateScope());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

