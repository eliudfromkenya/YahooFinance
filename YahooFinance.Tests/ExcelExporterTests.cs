using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YahooFinance.Core.Models;
using YahooFinance.Infrastructure.Services;
using Xunit;

namespace YahooFinance.Tests;

public class ExcelExporterTests
{
    [Fact]
    public async Task ExportAsync_WritesFile()
    {
        var exporter = new ExcelExporter();
        var tmp = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.xlsx");
        var quotes = new []
        {
            new HistoricalQuote { Date = DateTime.Today, Open = 1, High = 2, Low = 0.5m, Close = 1.5m, AdjustedClose = 1.5m, Volume = 1000 }
        };

        await exporter.ExportAsync(quotes, tmp, CancellationToken.None);

        Assert.True(File.Exists(tmp));
        var fi = new FileInfo(tmp);
        Assert.True(fi.Length > 0);

        File.Delete(tmp);
    }
}

