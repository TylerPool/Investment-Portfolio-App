namespace Portfolio.Api.Models;

public class AccountSaveRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? AccountType { get; set; }
    public decimal? Cash { get; set; }
    public List<AccountTradeDto>? Trades { get; set; }

    public class AccountTradeDto
    {
        public int? Id { get; set; }
        public string? Symbol { get; set; }
        public int AccountId { get; set; }
        public int Quantity { get; set; }
        public string? OpenDate { get; set; }
        public double OpenPrice { get; set; }
        public string? CloseDate { get; set; }
        public double? ClosePrice { get; set; }
    }
}
