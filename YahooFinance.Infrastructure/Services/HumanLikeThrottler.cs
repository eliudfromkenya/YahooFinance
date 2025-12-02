using YahooFinance.Core.Abstractions;

namespace YahooFinance.Infrastructure.Services;

public sealed class HumanLikeThrottler : IRequestThrottler
{
    private readonly TimeSpan minInterval;
    private readonly int jitterMillis;
    private DateTime lastTime;

    public HumanLikeThrottler(TimeSpan? minInterval = null, int jitterMillis = 400)
    {
        this.minInterval = minInterval ?? TimeSpan.FromSeconds(2);
        this.jitterMillis = jitterMillis;
        lastTime = DateTime.UtcNow.AddSeconds(-10);
    }

    public async Task WaitAsync(CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow - lastTime;
        var wait = minInterval - since;
        if (wait < TimeSpan.Zero)
        {
            wait = TimeSpan.Zero;
        }
        var rnd = new Random();
        var jitter = TimeSpan.FromMilliseconds(rnd.Next(0, jitterMillis));
        var total = wait + jitter;
        if (total > TimeSpan.Zero)
        {
            await Task.Delay(total, cancellationToken).ConfigureAwait(false);
        }
        lastTime = DateTime.UtcNow;
    }
}

