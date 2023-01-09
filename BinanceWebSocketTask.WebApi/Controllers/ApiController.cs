using BinanceWebSocketTask.Business.Mediator.Queries;
using BinanceWebSocketTask.Business.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BinanceWebSocketTask.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApiController(IMediator mediator) =>
        _mediator = mediator;

    [HttpGet("{symbol}/24hAvgPrice")]
    public async Task<ActionResult<Symbol24hAveragePriceModel>> Get(string symbol)
    {
        return await this._mediator.Send(new GetSymbol24hAveragePriceQuery { Symbol = symbol });
    }
    
    [HttpGet("{symbol}/SimpleMovingAverage")]
    public async Task<ActionResult<SymbolSimpleMovingAveragePriceModel>> Get(
        string symbol,
        [FromQuery(Name = "n")]int numberOfDataPoints,
        [FromQuery(Name = "p")]string dataPointsTimeInterval,
        [FromQuery(Name = "s")]string? dataPointsEndDate)
    {
        return await this._mediator.Send(new GetSymbolSimpleMovingAveragePriceQuery
        {
            Symbol = symbol,
            DataPointsTimeInterval = dataPointsTimeInterval,
            NumberOfDataPoints = numberOfDataPoints,
            DataPointsEndDate = dataPointsEndDate
        });
    }
}