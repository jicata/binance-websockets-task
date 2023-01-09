namespace BinanceWebSocketTask.Business.Models;

public abstract class BaseModel
{
    public string Errors { get; set; }

    public string Message { get; set; }
}