using YahooFinance.Core.Abstractions;
using YahooFinance.Core.Models;

namespace YahooFinance.Infrastructure.Services;

public sealed class StatsCalculator : IStatsCalculator
{
    public StatsResult Calculate(IEnumerable<HistoricalQuote> quotes)
    {
        var list = quotes.ToList();
        if (list.Count == 0)
        {
            return new StatsResult { Count = 0 };
        }

        decimal avg = list.Average(q => q.Close);
        decimal min = list.Min(q => q.Close);
        decimal max = list.Max(q => q.Close);
        decimal variance = list.Average(q => (q.Close - avg) * (q.Close - avg));
        decimal stdDev = (decimal)Math.Sqrt((double)variance);

        return new StatsResult
        {
            Count = list.Count,
            AverageClose = Math.Round(avg, 4),
            MinClose = min,
            MaxClose = max,
            StandardDeviationClose = Math.Round(stdDev, 4)
        };
    }
}

