namespace YahooFinance.Core.Models;

public sealed class StatsResult
{
    public int Count { get; init; }
    public decimal AverageClose { get; init; }
    public decimal MinClose { get; init; }
    public decimal MaxClose { get; init; }
    public decimal StandardDeviationClose { get; init; }
}

