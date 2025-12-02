using YahooFinance.Core.Models;

namespace YahooFinance.Core.Abstractions;

public interface IStatsCalculator
{
    StatsResult Calculate(IEnumerable<HistoricalQuote> quotes);
}

