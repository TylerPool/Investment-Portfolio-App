using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public class PortfolioStore
{
    private readonly List<Trade> _trades = new();
    private int _nextId = 1;

    private readonly Dictionary<string,double> _mockQuotes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AAPL"] = 230.10,
        ["MSFT"] = 452.55,
        ["GOOGL"] = 182.30,
        ["NVDA"] = 120.40,
        ["TSLA"] = 265.75
    };

    public IEnumerable<Trade> GetTrades() => _trades;

    // Trades by account: include static demo rows + any added trades for that account
    public IEnumerable<Trade> GetTradesByAccount(int accountId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var results = new List<Trade>();

        if (accountId == 1)
        {
            results.Add(new Trade { Id = 1001, AccountId = 1, Symbol = "AAPL", Quantity = 30, OpenDate = today, OpenPrice = 100 });
        }
        else if (accountId == 2)
        {
            results.Add(new Trade { Id = 2001, AccountId = 2, Symbol = "TSLA", Quantity = 30, OpenDate = today, OpenPrice = 200 });
        }

        results.AddRange(_trades.Where(t => t.AccountId == accountId));
        return results;
    }

    public Trade AddTrade(Trade t)
    {
        t.Id = _nextId++;
        _trades.Add(t);
        return t;
    }

    public bool DeleteTrade(int id)
    {
        var idx = _trades.FindIndex(x => x.Id == id);
        if (idx < 0) return false;
        _trades.RemoveAt(idx);
        return true;
    }

    public double? GetQuote(string symbol)
        => _mockQuotes.TryGetValue(symbol, out var p) ? p : null;

    public IEnumerable<Position> GetPositions()
    {
        var groups = _trades.GroupBy(t => t.Symbol, StringComparer.OrdinalIgnoreCase);
        foreach (var g in groups)
        {
            var qty = g.Sum(x => x.Quantity);
            var avgCost = g.Any() ? g.Average(x => x.OpenPrice) : 0;
            var price = GetQuote(g.Key);
            yield return new Position
            {
                Symbol = g.Key.ToUpperInvariant(),
                Quantity = qty,
                AvgCost = Math.Round(avgCost, 2),
                MarketPrice = price
            };
        }
    }
}
