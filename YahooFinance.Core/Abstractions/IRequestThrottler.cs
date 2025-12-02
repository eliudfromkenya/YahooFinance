namespace YahooFinance.Core.Abstractions;

public interface IRequestThrottler
{
    Task WaitAsync(CancellationToken cancellationToken);
}

