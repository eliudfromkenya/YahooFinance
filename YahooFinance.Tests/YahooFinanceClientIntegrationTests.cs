using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YahooFinance.Infrastructure.Services;
using Xunit;

namespace YahooFinance.Tests;

public class YahooFinanceClientIntegrationTests
{
    [Fact]
    public async Task GetHistoricalAsync_ReturnsData_ForKnownSymbol()
    {
        using var http = new HttpClient();
        var client = new YahooFinanceClient(http);
        var data = await client.GetHistoricalAsync("MSFT", "1mo", CancellationToken.None);

        Assert.NotEmpty(data);
    }
}

