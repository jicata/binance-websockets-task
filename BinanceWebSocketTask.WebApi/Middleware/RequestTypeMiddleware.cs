namespace BinanceWebSocketTask.WebApi.Middleware;

public class RequestTypeMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTypeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.ContentType == "application/xml")
        {
            httpContext.Request.Headers.Accept = "application/xml";
        }
        else
        {
            httpContext.Request.Headers.Accept = "application/json";
        }
        return _next(httpContext);
    }
}