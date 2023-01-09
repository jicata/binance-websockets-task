using BinanceWebSocketTask.Data.Entities.Enums;

namespace BinanceWebSocketTask.Data;

public static class Constants
{
    public const string DATE_FORMAT_GET_SIMPLE_MOVING_AVERAGE_QUERY = "yyyy-MM-dd";

    public static Dictionary<string, SimpleMovingAverageDataPointTimeInterval> TimeIntervalConversionMap = new()
    {
        { "1m", SimpleMovingAverageDataPointTimeInterval.OneMinute },
        { "5m", SimpleMovingAverageDataPointTimeInterval.FiveMinutes },
        { "30m", SimpleMovingAverageDataPointTimeInterval.ThirtyMinutes },
        { "1d", SimpleMovingAverageDataPointTimeInterval.OneDay },
        { "1w", SimpleMovingAverageDataPointTimeInterval.OneWeek }
    };
}