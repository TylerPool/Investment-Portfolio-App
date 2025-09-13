namespace Portfolio.Api.Models;

public enum AccountType
{
    TaxableBrokerage = 1,
    RothIra = 2
}
public class Account
{
    public string Id { get; set; }
    public AccountType AccountType { get; set; }
    public decimal Cash { get; set; } = 0;
    public List<Trade> Trades { get; set; } = new List<Trade>();
    public Account(string id, AccountType accountType)
    {
        this.Id = id;
        this.AccountType = accountType;
    }
    public override string ToString()
    {
        return $"Account ID: {this.Id}";
    }
}