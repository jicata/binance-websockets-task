namespace BinanceWebSocketTask.Business.Services.Contracts;

public interface IDateService : IService
{
    DateTime GetUtcNow();
}