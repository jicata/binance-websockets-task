using System.Globalization;
using BinanceWebSocketTask.Business.Mediator.Queries;
using FluentValidation;
using static BinanceWebSocketTask.Business.Constants;

namespace BinanceWebSocketTask.Business.Validation;

public class GetSymbolSimpleAveragePriceQueryValidator : AbstractValidator<GetSymbolSimpleMovingAveragePriceQuery>
{
    public GetSymbolSimpleAveragePriceQueryValidator()
    {
        RuleFor(x => x.DataPointsTimeInterval).Must(x => TimeIntervalConversionMap.ContainsKey(x))
            .WithMessage($"Time interval is not recognized");
        RuleFor(x => x.DataPointsEndDate)
            .Must(x => DateTime.TryParseExact(
                x,
                DATE_FORMAT_GET_SIMPLE_MOVING_AVERAGE_QUERY,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
            .When(x => !string.IsNullOrEmpty(x.DataPointsEndDate))
            .WithMessage($"Date time supplied is not in the valid format \"{DATE_FORMAT_GET_SIMPLE_MOVING_AVERAGE_QUERY}\"");
    }
}