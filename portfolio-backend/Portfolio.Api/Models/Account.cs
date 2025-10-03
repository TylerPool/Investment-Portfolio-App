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
    const string ACCOUNT_FILE_PATH = "/Users/tylerpool/Developer/files/accounts";
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
        string fileName = ACCOUNT_FILE_PATH + "/" + Id + ".xml";
        if (!File.Exists(fileName))
        {
            return;
        }

        var document = XDocument.Load(fileName);
        var root = document.Element("Account");
        if (root is null)
        {
            return;
        }

        var idValue = root.Element("Id")?.Value;
        if (!string.IsNullOrWhiteSpace(idValue))
        {
            Id = idValue;
        }

        var accountTypeValue = root.Element("AccountType")?.Value;
        if (!string.IsNullOrWhiteSpace(accountTypeValue))
        {
            if (Enum.TryParse<AccountType>(accountTypeValue, true, out var parsedAccountType))
            {
                AccountType = parsedAccountType;
            }
            else if (int.TryParse(accountTypeValue, out var accountTypeNumeric) && Enum.IsDefined(typeof(AccountType), accountTypeNumeric))
            {
                AccountType = (AccountType)accountTypeNumeric;
            }
        }

        var cashValue = root.Element("Cash")?.Value;
        if (!string.IsNullOrWhiteSpace(cashValue)
            && decimal.TryParse(cashValue, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var cash))
        {
            Cash = cash;
        }

        var tradesElement = root.Element("Trades");
        var trades = new List<Trade>();
        if (tradesElement is not null)
        {
            foreach (var tradeElement in tradesElement.Elements("Trade"))
            {
                var tradeIdText = tradeElement.Element("Id")?.Value;
                var symbol = tradeElement.Element("Symbol")?.Value ?? string.Empty;
                var accountIdText = tradeElement.Element("AccountId")?.Value;
                var quantityText = tradeElement.Element("Quantity")?.Value;
                var openDateText = tradeElement.Element("OpenDate")?.Value;
                var openPriceText = tradeElement.Element("OpenPrice")?.Value;
                var closeDateText = tradeElement.Element("CloseDate")?.Value;
                var closePriceText = tradeElement.Element("ClosePrice")?.Value;

                if (!int.TryParse(tradeIdText, out var tradeId)
                    || !int.TryParse(accountIdText, out var tradeAccountId)
                    || !int.TryParse(quantityText, out var quantity)
                    || !DateOnly.TryParseExact(openDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var openDate)
                    || !double.TryParse(openPriceText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var openPrice))
                {
                    continue;
                }

                var trade = new Trade(tradeId, symbol, tradeAccountId, quantity, openDate, openPrice);

                if (!string.IsNullOrWhiteSpace(closeDateText)
                    && DateOnly.TryParseExact(closeDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var closeDate))
                {
                    trade.CloseDate = closeDate;
                }

                if (!string.IsNullOrWhiteSpace(closePriceText)
                    && double.TryParse(closePriceText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var closePrice))
                {
                    trade.ClosePrice = closePrice;
                }

                trades.Add(trade);
            }
        }

        Trades = trades;
    }
    public override string ToString()
    {
        return $"Account ID: {this.Id}";
    }
}
