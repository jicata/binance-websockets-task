using BinanceWebSocketTask.Business.Services.Contracts;

namespace BinanceWebSocketTask.Business.Services;

public class DateService : IDateService
{
    public DateTime GetUtcNow() => DateTime.UtcNow;
}