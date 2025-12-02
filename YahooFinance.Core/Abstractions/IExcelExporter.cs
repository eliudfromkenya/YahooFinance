using YahooFinance.Core.Models;

namespace YahooFinance.Core.Abstractions;

public interface IExcelExporter
{
    Task ExportAsync(IEnumerable<HistoricalQuote> quotes, string filePath, CancellationToken cancellationToken);
}

