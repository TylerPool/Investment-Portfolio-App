namespace Portfolio.Api.Models;

public class Portfolio
{
    public string Id { get; set; }
    public List<Account> Accounts { get; set; }
    public Portfolio(string id)
    {
        this.Id = id;
        this.Accounts = new List<Account>();
    }
    public override string ToString()
    {
        return $"Portfolio ID: {this.Id}";
    }

    public void PopulateDummyPortfolio()
    {
        List<Trade> dummyTradesTaxable = new List<Trade>([new Trade(1,"AAPL",
            1,
            10,
            DateOnly.Parse("01/01/2024"),
            100)]);
        Account dummyTaxable = new Account("A-001", AccountType.TaxableBrokerage);
        dummyTaxable.Trades = (dummyTradesTaxable);
        List<Trade> dummyTradesRoth = new List<Trade>([new Trade(1,"TSLA",
            1,
            20,
            DateOnly.Parse("02/01/2024"),
            100)]);
        Account dummyRoth = new Account("A-002", AccountType.RothIra);
        dummyRoth.Trades = dummyTradesRoth;
        this.Accounts.Add(dummyTaxable);
        this.Accounts.Add(dummyRoth);

    }
}