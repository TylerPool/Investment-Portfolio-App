namespace Portfolio.Api.Models;

public class Position
{
    public string Symbol { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double AvgCost { get; set; }
    public double? MarketPrice { get; set; }
    public double? MarketValue => MarketPrice.HasValue ? Quantity * MarketPrice : null;
    public double UnrealizedPnL => MarketPrice.HasValue ? Quantity * (MarketPrice.Value - AvgCost) : 0;
}