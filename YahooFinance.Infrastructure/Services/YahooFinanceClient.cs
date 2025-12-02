using System.Net.Http;
using System.Text.Json;
using YahooFinance.Core.Abstractions;
using YahooFinance.Core.Models;

namespace YahooFinance.Infrastructure.Services;

public sealed class YahooFinanceClient : IYahooFinanceClient
{
    private readonly HttpClient httpClient;
    private readonly IRequestThrottler throttler;

    public YahooFinanceClient(HttpClient httpClient, IRequestThrottler throttler)
    {
        this.httpClient = httpClient;
        this.throttler = throttler;
        this.httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
        this.httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
    }

    public async Task<IReadOnlyList<HistoricalQuote>> GetHistoricalAsync(string symbol, string range, CancellationToken cancellationToken)
    {
        var url = $"https://query1.finance.yahoo.com/v8/finance/chart/{Uri.EscapeDataString(symbol)}?interval=1d&range={Uri.EscapeDataString(range)}&events=div%2Csplit";
        await throttler.WaitAsync(cancellationToken).ConfigureAwait(false);
        using var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var doc = await JsonSerializer.DeserializeAsync<JsonElement>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken).ConfigureAwait(false);

        var result = new List<HistoricalQuote>();

        var chart = doc.GetProperty("chart");
        var errorPropExists = chart.TryGetProperty("error", out var error) && error.ValueKind != JsonValueKind.Null;
        if (errorPropExists)
        {
            throw new InvalidOperationException(error.GetProperty("description").GetString() ?? "Unknown error");
        }

        var resultObj = chart.GetProperty("result")[0];
        var timestamps = resultObj.GetProperty("timestamp");
        var indicators = resultObj.GetProperty("indicators").GetProperty("quote")[0];

        var opens = indicators.GetProperty("open");
        var highs = indicators.GetProperty("high");
        var lows = indicators.GetProperty("low");
        var closes = indicators.GetProperty("close");
        var volumes = indicators.GetProperty("volume");

        for (int i = 0; i < timestamps.GetArrayLength(); i++)
        {
            var ts = timestamps[i].GetInt64();
            var date = DateTimeOffset.FromUnixTimeSeconds(ts).DateTime;

            decimal GetDecimalOrZero(JsonElement el) => el.ValueKind == JsonValueKind.Number ? (decimal)el.GetDouble() : 0m;
            long GetLongOrZero(JsonElement el) => el.ValueKind == JsonValueKind.Number ? el.GetInt64() : 0L;

            var quote = new HistoricalQuote
            {
                Date = date,
                Open = GetDecimalOrZero(opens[i]),
                High = GetDecimalOrZero(highs[i]),
                Low = GetDecimalOrZero(lows[i]),
                Close = GetDecimalOrZero(closes[i]),
                AdjustedClose = GetDecimalOrZero(closes[i]),
                Volume = GetLongOrZero(volumes[i])
            };

            result.Add(quote);
        }

        return result;
    }
}

