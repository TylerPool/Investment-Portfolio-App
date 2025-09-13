using Microsoft.AspNetCore.Http.Features;

namespace Portfolio.Api.Models;

public class Trade
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public int AccountId { get; set; }
    public int Quantity { get; set; }
    public DateOnly OpenDate { get; set; }
    public double OpenPrice { get; set; }
    public DateOnly? CloseDate { get; set; }
    public double? ClosePrice { get; set; }

    public Trade(int id, string symbol, int accountId, int quantity, DateOnly openDate, double openPrice)
    {
        Id = id;
        Symbol = symbol ?? string.Empty;
        AccountId = accountId;
        Quantity = quantity;
        OpenDate = openDate;
        OpenPrice = openPrice;
    }
    public override string ToString()
    {
        return $"Trade ID: {this.Id}";
    }
    public static string foo()
    {
        // for early testing only
        return "bar";
    }

}
