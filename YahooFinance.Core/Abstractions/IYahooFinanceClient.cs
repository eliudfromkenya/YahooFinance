using YahooFinance.Core.Models;

namespace YahooFinance.Core.Abstractions;

public interface IYahooFinanceClient
{
    Task<IReadOnlyList<HistoricalQuote>> GetHistoricalAsync(string symbol, string range, CancellationToken cancellationToken);
}

