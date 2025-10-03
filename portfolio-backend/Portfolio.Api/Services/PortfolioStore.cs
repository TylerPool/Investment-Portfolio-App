
using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public class PortfolioStore
{
    private readonly List<Trade> _trades = new();
    private int _nextId = 1;

    private readonly Dictionary<string, double> _mockQuotes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AAPL"] = 230.10,
        ["MSFT"] = 452.55,
        ["GOOGL"] = 182.30,
        ["NVDA"] = 120.40,
        ["TSLA"] = 265.75
    };

    public Portfolio.Api.Models.Portfolio GetPortfolio(string id)
    {
        // for now always populate with the dummy profile for testing. 
        // soon to be replaced with lookiup via CSV and eventually SQL table.
        Portfolio.Api.Models.Portfolio dummyPortfolio = new Portfolio.Api.Models.Portfolio("P-001");
        dummyPortfolio.PopulateDummyPortfolio();
        return dummyPortfolio;
    }
    public Account GetAccount(string id)
    {
        Account account = new Account(id);
        account.Load();
        return account;

        //place holder method for now that returns dummy accounts
        // Account a = new Account(id);
        // var today = DateOnly.FromDateTime(DateTime.Today);

        // if (id == "1")
        // {
        //     a.AccountType = AccountType.TaxableBrokerage;
        //     a.Trades.Add(new Trade(1001, "AAPL", 1, 30, today, 100));
        // }
        // else if (id == "2")
        // {
        //     a.AccountType = AccountType.RothIra;
        //     a.Trades.Add(new Trade(1001, "TSLA", 1, 30, today, 100));
        // }

        // return a;
    }

    public double? GetQuote(string symbol) => _mockQuotes.TryGetValue(symbol, out var p) ? p : null;
    public void SaveAccount(Account account)
    {
        account.Save();
    }
    public void SaveAccountTest()
    {
        Portfolio.Api.Models.Portfolio dummyPortfolio = GetPortfolio("1");
        dummyPortfolio.Accounts[0].Save();
    }

}