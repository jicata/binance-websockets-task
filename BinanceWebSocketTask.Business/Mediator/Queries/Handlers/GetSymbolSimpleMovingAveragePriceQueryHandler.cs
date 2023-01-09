using BinanceWebSocketTask.Business.Models;
using BinanceWebSocketTask.Business.Services.Contracts;
using FluentValidation;
using MediatR;

namespace BinanceWebSocketTask.Business.Mediator.Queries.Handlers;

public class GetSymbolSimpleMovingAveragePriceQueryHandler : 
    IRequestHandler<GetSymbolSimpleMovingAveragePriceQuery, SymbolSimpleMovingAveragePriceModel>
{
    private readonly IValidator<GetSymbolSimpleMovingAveragePriceQuery> _validator;
    private readonly ISymbolSimpleMovingAverageDataPointService _symbolSimpleMovingAverageService;
    
    public GetSymbolSimpleMovingAveragePriceQueryHandler(
        IValidator<GetSymbolSimpleMovingAveragePriceQuery> validator,
        ISymbolSimpleMovingAverageDataPointService symbolSimpleMovingAverageService)
    {
        _validator = validator;
        _symbolSimpleMovingAverageService = symbolSimpleMovingAverageService;
    }
    
    public async Task<SymbolSimpleMovingAveragePriceModel> Handle(GetSymbolSimpleMovingAveragePriceQuery request, CancellationToken cancellationToken)
    {
        var result = await this._validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            return new SymbolSimpleMovingAveragePriceModel
            {
                Errors = string.Join($"{Environment.NewLine}", result.Errors.Select(e => e.ErrorMessage))
            };
        }

        return this._symbolSimpleMovingAverageService.GetSymbolSimpleMovingAverage(request);
    }
}