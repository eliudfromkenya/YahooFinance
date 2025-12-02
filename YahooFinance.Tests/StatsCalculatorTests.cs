using System;
using YahooFinance.Infrastructure.Services;
using YahooFinance.Core.Models;
using Xunit;

namespace YahooFinance.Tests;

public class StatsCalculatorTests
{
    [Fact]
    public void Calculate_ReturnsExpectedStats()
    {
        var calc = new StatsCalculator();
        var quotes = new []
        {
            new HistoricalQuote { Date = DateTime.Today.AddDays(-2), Close = 100m },
            new HistoricalQuote { Date = DateTime.Today.AddDays(-1), Close = 110m },
            new HistoricalQuote { Date = DateTime.Today, Close = 90m },
        };

        var stats = calc.Calculate(quotes);

        Assert.Equal(3, stats.Count);
        Assert.Equal(100m, stats.AverageClose);
        Assert.Equal(90m, stats.MinClose);
        Assert.Equal(110m, stats.MaxClose);
        Assert.True(stats.StandardDeviationClose > 0);
    }
}

