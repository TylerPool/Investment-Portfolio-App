using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

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
    const string ACCOUNT_FILE_PATH = "/Users/tyler/Developer/files/accounts";
    public Account(string id, AccountType accountType)
    {
        this.Id = id;
        this.AccountType = accountType;
    }
    public Account(string id) { Id = id; }
    public void Save()
    {
        // Early solution for saving results. Someday replaced with DB call
        string fileName = ACCOUNT_FILE_PATH + "/" + Id + ".xml";
        var directory = Path.GetDirectoryName(fileName);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var tradesElement = new XElement("Trades",
            (Trades ?? new List<Trade>()).Select(trade => new XElement("Trade",
                new XElement("Id", trade.Id),
                new XElement("Symbol", trade.Symbol ?? string.Empty),
                new XElement("AccountId", trade.AccountId),
                new XElement("Quantity", trade.Quantity),
                new XElement("OpenDate", trade.OpenDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new XElement("OpenPrice", trade.OpenPrice.ToString(CultureInfo.InvariantCulture)),
                new XElement("CloseDate", trade.CloseDate.HasValue
                    ? trade.CloseDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : string.Empty),
                new XElement("ClosePrice", trade.ClosePrice.HasValue
                    ? trade.ClosePrice.Value.ToString(CultureInfo.InvariantCulture)
                    : string.Empty)
            ))
        );
        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("Account",
                new XElement("Id", Id ?? string.Empty),
                new XElement("AccountType", AccountType.ToString()),
                new XElement("Cash", Cash.ToString(CultureInfo.InvariantCulture)),
                tradesElement
            )
        );
        document.Save(fileName);
    }

    public void Load()
    {

    }
    public override string ToString()
    {
        return $"Account ID: {this.Id}";
    }
}
