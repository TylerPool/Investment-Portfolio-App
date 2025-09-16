using Portfolio.Api.Models;
using Portfolio.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Allow Angular dev server
const string CorsPolicy = "AllowAngular";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

builder.Services.AddSingleton<PortfolioStore>();

var app = builder.Build();
app.UseCors(CorsPolicy);

// Optional: fix port for predictability
app.Urls.Add("http://localhost:5200");

app.MapGet("/api/health", () => Results.Ok(new { ok = true }));

app.MapGet("/api/trades", (PortfolioStore store) => Results.Ok(store.GetTrades()));

// New: trades by account (dummy data in PortfolioStore)
app.MapGet("/api/accounts/{accountId:int}/trades", (PortfolioStore store, int accountId)
    => Results.Ok(store.GetTradesByAccount(accountId)));

app.MapPost("/api/trades", (PortfolioStore store, Trade trade) =>
{
    if (string.IsNullOrWhiteSpace(trade.Symbol) || trade.Quantity == 0 || trade.OpenPrice <= 0)
        return Results.BadRequest("Symbol, Quantity and OpenPrice are required");

    trade.Symbol = trade.Symbol.Trim().ToUpperInvariant();
    var created = store.AddTrade(trade);
    return Results.Created($"/api/trades/{created.Id}", created);
});

app.MapDelete("/api/trades/{id:int}", (PortfolioStore store, int id) =>
    store.DeleteTrade(id) ? Results.NoContent() : Results.NotFound());

app.MapGet("/api/positions", (PortfolioStore store) => Results.Ok(store.GetPositions()));

app.MapGet("/api/quotes/{symbol}", (PortfolioStore store, string symbol) =>
{
    var price = store.GetQuote(symbol);
    return price is null ? Results.NotFound() : Results.Ok(new { symbol = symbol.ToUpperInvariant(), price });
});

app.MapGet("/api/portfolio/{id}",(PortfolioStore store, string id) =>
{
    var portfolio = store.GetPortfolio(id);
    return portfolio is null ? Results.NotFound() : Results.Ok(portfolio);
});

app.Run("http://localhost:5200");

// -------- THIS WORKS! ------------
// var builder = WebApplication.CreateBuilder(args);

// // Allow CORS (so Angular frontend can call later)
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//     });
// });

// var app = builder.Build();

// app.UseCors();

// // Health check endpoint
// app.MapGet("/api/health", () => Results.Ok(new { ok = true }));

// // Example endpoint for trades
// app.MapGet("/api/trades", () =>
// {
//     return new[] {
//         new { Symbol = "AAPL", Quantity = 10, Price = 150.0 },
//         new { Symbol = "MSFT", Quantity = 5, Price = 320.0 }
//     };
// });

// app.MapGet("/", () => "Portfolio API is running!"); // 👈 add this to avoid 404 at root

// app.Run("http://localhost:5200");
