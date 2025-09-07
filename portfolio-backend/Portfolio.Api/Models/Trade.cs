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

    public static string foo()
    {
        return "bar";
    }

}
